﻿namespace API.DTO.Responses.Pages.HomePage
{
    public record HomeProjectLeader
    {
        public int ProjectLeaderId { get; set; }
        public string ProjectLeaderName { get; set;}
        public string ProjectLeaderImageUrl { get; set; }
    }
}
