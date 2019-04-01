# Update 2.5
- End of Service

# Update 2.4
- Fix an issue regarding storing tuning settings

# Update 2.3
- Switch over to HTTPS

# Update 2.2
- Fix agent comparison not working when visiting the page with agents already selected

# Update 2.1
- Add unique agents counts per day and month
- Reorder badges, showing completed last and those currently in-progress by progress descending
- Fix badges being wrongfully assigned
- Fix a few internal errors

# Update 2.0
### Additions
- Site usage tracking**
- XpPerMatch for agents
- Agent comparison highlighting, so you can now more easily determine who wasted...I mean played the game the most
- Proper browser tab titles
- New "agents per month" graph
- New agent badges (basically petty achievements, but I call them badges)
- Semi-added mobile navigation, layout still sucks on small screens though, so don't expect too much
- Some fancy spinners here and there to give you something to look at while you wait for whatever the action you just performed is supposed to do
- Steam login and agent claiming***
- Dashboard showing maps, tunings, friends and not much more (might be a tad empty if you have none of those :fire:)
- Allow saving tuning settings and sharing them with others (or just keep them all to yourself)
- Allow rating maps and tunings of other agents (not your own obviously)
- Server pages now also show the reflective damage type, the room owner and the custom team names (if present and/or I haven't botched it again)
- Tuning is now bigger (and better, maybe) than ever, too bad I don't know what half of the settings do and the other half doesn't seem to work, but have fun anyway
- Track and display the last time an agent was seen online (Maximum Stalking). Also allow sorting agents by this property, because why not

### Fixes

- Navbar logo link no longer results in the page reloading
- Prevent graph tooltips from remaining visible when switching pages
- Improve graph tooltip widths...and timestamps...and positioning. You know what, just improve the whole damn thing
- Remove the trailing '#' in the page footer
- Tuning columns have been swapped and are now the right way round
- Clarify which changelog is shown in the navbar dropdown (it's not mine, that's for sure)
- Empty lists/tables now actually tell you that they are empty, hooray! They also shouldn't throw any more errors, double hooray!
- Stats page now actually shows the top agents by ratings again (**SPOILER**: It's still sebastianthecrab, what a surprise)
- Change the used colour scheme, bye-bye ugly green buttons
- Dropdowns didn't always drop down, now they drop 99.9% of the time
- Change navbar links and layout, intruder stuff is now in the footer, tuning is no longer in the dropdown
- Passworded servers are now correctly marked as passworded
- Change icons for the first two columns of the server list
- Percentages now have the "%"
- Asia and Japan now have flags (technically they always had flags, they just weren't set correctly)
- Matchmode settings (specifically the competitive ones) no longer crash the server monitoring job
- Imported agents are no longer marked as "Demoted" (whoops)
- Tweak the map popularity pie chart (no more 0%, sorry deathrunalpha)
- Few more visual tweaks here and there

### API and Boring stuff
- Add the API documentation link to both the footer as well as the navbar dropdown
- Add all that legal garbage you're required to have when hosting a website
- Improve memory usage for the API and the website
- Upgraded to .NET Core 2 and Angular 6 
- New documentation layout and design
- Server lists now also return the online agents (previously always an empty array)
- Agent lists now also contain the ratings, no need to send an extra query to retrieve them
- Changed the way agent updating works, should improve performance
- Online agents and servers are fetched every minute, not every 3 minutes as before (Bloon should benefit from this change)
- Maps are now linked to the author via the internal agent id and not the name

** The obtained data is completely anonymous and stored in-house, no third parties (e.g. Google Analytics) involved
*** Make sure to claim your *OWN* account

# Update 1.1
- Fixed the 24 hour peak statistic always showing 10 (it should've been more and it now is)
- Fixed the agent table's # index not counting properly
- Agent profiles now not only show the copy-pasted stats, these are now also compared to the corresponding averages
- Added support for tracking agent roles (AUG, Demoted, etc.)
- Reset the four agents with "modified" stats (sorry Austin, you're no longer in first place)
- Added agent flagging, which results in agents no longer being tracked (goobye forever)
- Split up some stats to allow sorting by these in the agent list, added average match length
- The server pages now also show the names of playing agents and the map cycle now links to the individual maps
- Improved inpage linking (makes it easier to quickly find stuff)
- And a few small visual fixes, mainly due to long agent names (dammit sebastian!)