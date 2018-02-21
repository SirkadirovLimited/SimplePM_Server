/*
 * Copyright (C) 2017, Kadirov Yurij.
 * All rights are reserved.
 * Licensed under Apache License 2.0 with additional restrictions.
 * 
 * @Author: Kadirov Yurij
 * @Website: https://sirkadirov.com/
 * @Email: admin@sirkadirov.com
 * @Repo: https://github.com/SirkadirovTeam/SimplePM_Server
 */

namespace SubmissionInfo
{

    public class SubmissionInfo
    {

        public int SubmissionId { get; set; }
        public int UserId { get; set; }
        public int ClassworkId { get; set; }
        public int OlympId { get; set; }

        public string CodeLang { get; set; }
        public string TestType { get; set; }
        public string CustomTest { get; set; }

        public byte[] ProblemCode { get; set; }

        public ProblemInfo ProblemInformation;
        
    }

    public class ProblemInfo
    {

        public int ProblemId { get; set; }
        public int ProblemDifficulty { get; set; }
        public bool AdaptProgramOutput { get; set; }

    }

}
