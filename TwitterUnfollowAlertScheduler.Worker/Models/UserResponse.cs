using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TwitterUnfollowAlertScheduler.Worker.Models
{
    public class UserResponse
    {
        public UserResponse()
        {
            UserIds = new List<long>();
        }

        [JsonProperty("ids")]
        public List<long> UserIds { get; set; }

        [JsonProperty("next_cursor")]
        public long NextCursor { get; set; }

        [JsonProperty("next_cursor_str")]
        public string NextCursorStr { get; set; }

        [JsonProperty("previous_cursor")]
        public long PreviousCursor { get; set; }

        [JsonProperty("previous_cursor_str")]
        public string PreviousCursorStr { get; set; }

        [NotMapped]
        public long TotalCount { get; set; }
    }
}
