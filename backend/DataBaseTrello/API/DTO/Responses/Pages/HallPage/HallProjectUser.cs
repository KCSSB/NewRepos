﻿namespace API.DTO.Responses.Pages.HallPage
{
    public record HallProjectUser
    {
        public int ProjectUserId { get; set; }
        public int UserId { get; set; }
        public string userUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProjectRole { get; set; }

    }
}
