using System;
using System.Text.Json.Serialization;
using Twitch.Net.EventSub.Models;

namespace Twitch.Net.EventSub.Notifications
{ 
   /**
    * THIS IS THE BASE MODEL
    * "id": "1243456",
    * "broadcaster_user_id": "1337",
    * "broadcaster_user_login": "cool_user",
    * "broadcaster_user_name": "Cool_User",
    * "title": "Aren’t shoes just really hard socks?",
    * "bits_voting": {
    *      "is_enabled": true,
    *      "amount_per_vote": 10
    * },
    * "channel_points_voting": {
    *      "is_enabled": true,
    *      "amount_per_vote": 10
    * },
    * "started_at": "2020-07-15T17:16:03.17106713Z",
    * "ends_at": "2020-07-15T17:16:08.17106713Z"
    */
    public abstract class ChannelPollBaseModel<T>
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterIdString { get; init; }
        public int BroadcasterId => int.Parse(BroadcasterIdString);

        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }

        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; }

        [JsonPropertyName("bits_voting")]
        public PointsVotingModel BitsVoting { get; init; }

        [JsonPropertyName("channel_points_voting")]
        public PointsVotingModel ChannelPointsVoting { get; init; }

        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; init; }

        [JsonPropertyName("ends_at")]
        public DateTime EndsAt { get; init; }
        
        // Not part of the base, but an easier way to implement the other ones.
        [JsonPropertyName("choices")]
        public PollBeginChoiceModel[] Choices { get; init; }
    }
}