// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using Newtonsoft.Json;

    public class FriendsList
    {
        [JsonProperty("friendslist")]
        public Friends Friends { get; private set; }
    }
}
