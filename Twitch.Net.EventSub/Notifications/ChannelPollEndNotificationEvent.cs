﻿using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications;

/**
* "id": "1243456",
* "broadcaster_user_id": "1337",
* "broadcaster_user_login": "cool_user",
* "broadcaster_user_name": "Cool_User",
* "title": "Aren’t shoes just really hard socks?",
* "choices": [
*      {"id": "123", "title": "Blue", "bits_votes": 50, "channel_points_votes": 70, "votes": 120},
*      {"id": "124", "title": "Yellow", "bits_votes": 100, "channel_points_votes": 40, "votes": 140},
*      {"id": "125", "title": "Green", "bits_votes": 10, "channel_points_votes": 70, "votes": 80}
* ],
* "bits_voting": {
*      "is_enabled": true,
*      "amount_per_vote": 10
* },
* "channel_points_voting": {
*      "is_enabled": true,
*      "amount_per_vote": 10
* },
* "status": "completed",
* "started_at": "2020-07-15T17:16:03.17106713Z",
* "ends_at": "2020-07-15T17:16:08.17106713Z"
*/
public class ChannelPollEndNotificationEvent : ChannelPollBaseModel<PollEndChoiceModel>
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
}

/**
 * "id": "123",
 * "title": "Yeah!",
 * "bits_votes": 50,
 * "channel_points_votes": 70,
 * "votes": 120
 */
public class PollEndChoiceModel : PollProgressChoiceModel
{
    // empty - nothing more added
}