using System.CodeDom;

namespace API.Constants
{
    public static class AllowLengthProp
    {
        //User
        public const int MinFirstUserName = 1;
        public const int MaxFirstUserName = 50;

        public const int MinLastUserName = 1;
        public const int MaxLasttUserName = 50;

        public const int MinEmail = 5;
        public const int MaxEmail = 254;

        public const int MinPassword = 6;
        public const int MaxPassword = 64;
        //User

        //Project
        public const int MinProjectName = 1;
        public const int MaxProjectName = 50;
        //Project

        //Board
        public const int MinBoardName = 1;
        public const int MaxBoardName = 50;
        //Board

        //Card
        public const int MinCardName = 1;
        public const int MaxCardName = 50;
        //Card

        //Task
        public const int MinTaskName = 1;
        public const int MaxTaskName = 50;

        public const int MinTaskDescription = 1;
        public const int MaxTaskDescription = 1000;
        //Task
    }
}
