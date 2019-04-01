// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using API.Models;
    using API.Utils;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Configuration;
    using NLog;

    public class AgentJobs
    {
        private static readonly Dictionary<string, AgentRatingType> VoteTypes = new Dictionary<string, AgentRatingType>()
        {
            { "general", AgentRatingType.General },
            { "communication",  AgentRatingType.Communication },
            { "nice", AgentRatingType.Niceness },
            { "teamplayer", AgentRatingType.Teamplayer },
            { "fair", AgentRatingType.Fairness }
        };

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration config;

        public AgentJobs(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<int> GetAgentAsync(int id, string agentName, bool flagged, AgentForumProfile profile = null)
        {
            // Initialize our agent with the passed parameters
            Agent agent = new Agent()
            {
                ID = id,
                Name = agentName,
                Flagged = flagged
            };

            // Check the database to see whether an agent with that name has already been added
            if (id == 0)
            {
                AgentJobDetails agentJobDetails = DB.Get(
                    "SELECT id, name, flagged FROM agent WHERE name = @name",
                    new Dictionary<string, dynamic>()
                    {
                        { "@name", agentName }
                    },
                    Create.AgentJobDetails);

                // Agent exists
                if (agentJobDetails != null)
                {
                    agent.ID = agentJobDetails.ID;
                    agent.Name = agentJobDetails.Name;
                    agent.Flagged = agentJobDetails.Flagged;

                    if (profile != null)
                    {
                        profile.ID = agent.ID;
                    }
                }
            }

            // Only retrieve stats, ratings and badges if the agent hasn't been flagged
            if (!agent.Flagged)
            {
                agent = await this.GetStats(agent).ConfigureAwait(false);

                // Agent not found
                if (agent == null || agent.ID == 0)
                {
                    return 0;
                }

                List<AgentRating> ratings = await this.GetRatings(agent).ConfigureAwait(false);

                if (ratings == null)
                {
                    agent.Ratings.AddRange(new List<AgentRating>());
                }
                else
                {
                    agent.Ratings.AddRange(ratings);
                }

                this.GetBadges(agent);
            }
            else
            {
                logger.Debug($"Skipping jobs for flagged agent {agentName}");
            }

            // Only get the forum profile if asked for
            if (profile == null)
            {
                profile = await this.GetForumProfile(agent.ID, agent.Name).ConfigureAwait(false);
            }

            this.UpdateAgentsForumProfile(profile);

            // Return the new or existing agent's id
            return agent.ID;
        }

        private async Task<Agent> GetStats(Agent agent)
        {
            logger.Debug($"Retrieving stats for agent {agent.Name}");

            HtmlDocument doc = await Request.GetHTMLAsync(new Uri(this.config["Settings:SBGURL"] + "/intruder/profile/?username=" + agent.Name)).ConfigureAwait(false);
            HtmlNode avatarNode = doc.DocumentNode.SelectSingleNode("//img[contains(@class, 'avatar-img')]");

            // Results from a redirect due to a bad username, gateway timeout, etc.
            if (avatarNode == null)
            {
                logger.Debug($"Unable to process {agent.Name}, aborting...");
                return null;
            }

            string avatar = avatarNode.Attributes["src"].Value;
            HtmlNodeCollection stats = doc.DocumentNode.SelectNodes("//table[contains(@class, 'stats-table')]/tbody/tr/td[@class='stats-td']");

            string[] times = stats[9].InnerText.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            string[] wins = stats[2].InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] survivals = stats[5].InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            agent.AvatarURL = new Uri(avatar.StartsWith("/", StringComparison.Ordinal) ? this.config["Settings:SBGURL"] + avatar : avatar);
            agent.XP = int.Parse(stats[0].InnerText.Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.TimePlayed = times[0] == "-"
                    ? 0
                    : (int.Parse(times[0].Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture) * 3600)
                        + (int.Parse(times[1].Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture) * 60);
            agent.MatchesPlayed = int.Parse(stats[1].InnerText.Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.MatchesWon = int.Parse(wins[0].Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.MatchesLost = int.Parse(stats[3].InnerText.Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.MatchesSurvived = int.Parse(survivals[0].Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.Arrests = int.Parse(stats[6].InnerText.Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.Captures = int.Parse(stats[7].InnerText.Replace(",", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture);
            agent.Status = (stats[8].InnerText == "offline") ? Status.Offline : Status.Online;
            agent.CurrentLocation = new Location()
            {
                Description = stats[8].InnerText
            };
            agent.LastUpdate = DateTime.Now;
            agent.MatchesTied = agent.MatchesPlayed - agent.MatchesWon - agent.MatchesLost;

            // Agent doesn't exist (SBG too lazy to make a proper 404 page) or didn't buy the game
            if (agent.ID == 0 && agent.XP == 0)
            {
                logger.Debug($"No XP for agent {agent.Name}, aborting...");
                return null;
            }

            Dictionary<string, dynamic> queryParameters = new Dictionary<string, dynamic>()
                {
                    { "@name", agent.Name },
                    { "@avatarURL", agent.AvatarURL },
                    { "@xp", agent.XP },
                    { "@timePlayed", agent.TimePlayed },
                    { "@matchesPlayed", agent.MatchesPlayed },
                    { "@matchesWon", agent.MatchesWon },
                    { "@matchesLost", agent.MatchesLost },
                    { "@matchesSurvived", agent.MatchesSurvived },
                    { "@arrests", agent.Arrests },
                    { "@captures", agent.Captures },
                    { "@status", agent.Status },
                    { "@currentLocation", agent.CurrentLocation.Description },
                    { "@lastUpdate", agent.LastUpdate }
                };

            if (agent.ID != 0)
            {
                queryParameters.Add("@id", agent.ID);
                DB.Update(
                    "UPDATE agent SET avatar_url = @avatarURL, xp = @xp, time_played = @timePlayed, matches_played = @matchesPlayed, matches_won = @matchesWon,"
                    + " matches_lost = @matchesLost, matches_survived = @matchesSurvived, arrests = @arrests, captures = @captures, status = @status,"
                    + " current_location = @currentLocation, last_update = @lastUpdate WHERE id = @id AND flagged = false",
                    queryParameters);
            }
            else
            {
                agent.ID = DB.Insert(
                    "INSERT INTO agent(name, avatar_url, xp, time_played, matches_played, matches_won, matches_lost, matches_survived, arrests, captures,"
                    + " status, current_location, last_update, last_seen, registered)"
                    + " VALUES (@name, @avatarURL, @xp, @timePlayed, @matchesPlayed, @matchesWon, @matchesLost, @matchesSurvived, @arrests, @captures,"
                    + " @status, @currentLocation, @lastUpdate, NOW(), NOW())",
                    queryParameters,
                    true);
            }

            logger.Debug($"Retrieved stats for agent {agent.Name}");

            return agent;
        }

        private void GetBadges(Agent agent)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "@id", agent.ID }
            };
            List<string> insertValues = new List<string>();

            List<AgentBadge> agentBadges = DB.GetList(
                "SELECT * FROM (SELECT badge.id, badge.title, badge.description, agent_id, agent_badge.progress FROM `agent_badge`"
                    + " INNER JOIN badge ON badge.id = agent_badge.badge_id WHERE agent_id = @id"
                    + " UNION SELECT badge.id, badge.title, badge.description, 0 AS agent_id, 0 AS progress FROM badge) badges"
                    + " GROUP BY id ",
                parameters,
                Create.AgentBadge);

            foreach (AgentBadge agentBadge in agentBadges)
            {
                // Skip completed badges
                if (agentBadge.Progress == 1)
                {
                    continue;
                }

                decimal progress = Util.CalculateBadgeProgress(agent, agentBadge.ID);

                if (progress > agentBadge.Progress)
                {
                    if (agentBadge.AgentID != 0)
                    {
                        DB.Update(
                            "UPDATE agent_badge SET progress = @progress WHERE agent_id = @agentID and badge_id = @badgeID",
                            new Dictionary<string, dynamic>()
                            {
                                { "@agentID", agentBadge.AgentID },
                                { "@badgeID", agentBadge.ID },
                                { "@progress", progress }
                            });
                    }
                    else
                    {
                        parameters.Add("@badge" + agentBadge.ID, agentBadge.ID);
                        parameters.Add("@progress" + agentBadge.ID, progress);
                        insertValues.Add($"(@id, @badge{agentBadge.ID}, @progress{agentBadge.ID})");
                    }
                }
            }

            if (parameters.Count > 1)
            {
                DB.Insert(
                $"INSERT INTO agent_badge(agent_id, badge_id, progress) VALUES {string.Join(",", insertValues.ToArray())}",
                parameters);
            }
        }

        private async Task<AgentForumProfile> GetForumProfile(int id, string agentName)
        {
            HtmlDocument doc = await Request.GetHTMLAsync(new Uri(this.config["Settings:SBGURL"] + "/forum/memberlist.php?sk=c&sd=a&joined_select=lt&count_select=eq&username=" + agentName)).ConfigureAwait(false);

            if (doc == null)
            {
                logger.Error($"Unable to parse the forum profile for agent {agentName}, aborting...");
                return new AgentForumProfile()
                {
                    ID = id,
                    Role = AgentRole.Agent,
                    Registered = DateTime.Now
                };
            }

            // rankNode is null for agents without a forum profile
            HtmlNode rankNode = doc.DocumentNode.SelectSingleNode("//table[@id='memberlist']/tbody/tr/td[1]/span[contains(@class, 'rank-img')]/img");

            AgentRole role = Util.GetConvertedRole(rankNode == null ? "Agent" : rankNode.Attributes["title"].Value);
            DateTime registrationTimestamp = rankNode == null ? DateTime.Now : DateTime.Parse(doc.DocumentNode.SelectSingleNode("//table[@id='memberlist']/tbody/tr/td[contains(@class, 'joined')]").InnerText, CultureInfo.InvariantCulture);

            return new AgentForumProfile()
            {
                ID = id,
                Role = role,
                Registered = registrationTimestamp
            };
        }

        private async Task<List<AgentRating>> GetRatings(Agent agent)
        {
            logger.Debug($"Retrieving ratings for agent {agent.Name}");

            List<AgentRating> ratings = new List<AgentRating>();
            Dictionary<string, dynamic> insertParameters = new Dictionary<string, dynamic>()
            {
                { "@id", agent.ID }
            };
            List<string> insertValues = new List<string>();

            for (int i = 0; i < VoteTypes.Count; i++)
            {
                string rating = await Request.PostAsync(
                    new Uri(this.config["Settings:SBGURL"] + "/intruder/profile/voteAttribute.php"),
                    new Dictionary<string, string>()
                    {
                        { "action", "getvoteanon" },
                        { "attribute", VoteTypes.ElementAt(i).Key },
                        { "username", agent.Name }
                    }).ConfigureAwait(false);

                string[] ratingParts = rating.Split(',');

                if (ratingParts.Length != 3)
                {
                    logger.Error($"Unable to process rating {rating} of type {VoteTypes.ElementAt(i).Key} for user {agent.Name}");
                    return null;
                }

                int voteStatus = string.IsNullOrEmpty(ratingParts[1]) ? 0 : int.Parse(ratingParts[1], CultureInfo.InvariantCulture);
                int totalRatings = string.IsNullOrEmpty(ratingParts[2]) ? 0 : int.Parse(ratingParts[2], CultureInfo.InvariantCulture);
                int positive = (voteStatus > 0) ? voteStatus : 0;
                int negative = (voteStatus < 0) ? Math.Abs(voteStatus) : 0;
                int remaining = totalRatings - Math.Abs(voteStatus);

                if (remaining > 0)
                {
                    negative += (int)Math.Floor((decimal)remaining / 2);
                    positive += (int)Math.Floor((decimal)remaining / 2);
                }

                insertParameters.Add("@type" + i, VoteTypes.ElementAt(i).Value.ToString());
                insertParameters.Add("@positive" + i, positive);
                insertParameters.Add("@negative" + i, negative);
                insertValues.Add($"(@id, @type{i}, @positive{i}, @negative{i})");
                ratings.Add(new AgentRating()
                {
                    AgentID = agent.ID,
                    Type = VoteTypes.ElementAt(i).Value,
                    Positive = positive,
                    Negative = negative
                });
            }

            DB.Insert(
                $"INSERT INTO agent_rating(agent_id, type, positive, negative) VALUES {string.Join(",", insertValues.ToArray())}"
                + " ON DUPLICATE KEY UPDATE positive = VALUES(positive), negative = VALUES(negative)",
                insertParameters);

            logger.Debug($"Retrieved stats for agent {agent.Name}");

            return ratings;
        }

        private void UpdateAgentsForumProfile(AgentForumProfile profile)
        {
            DB.Update(
                "UPDATE agent SET registered = @registered, role = @role WHERE id = @id",
                new Dictionary<string, dynamic>()
                {
                    { "role", profile.Role },
                    { "@registered", profile.Registered },
                    { "@id", profile.ID }
                });
        }
    }
}
