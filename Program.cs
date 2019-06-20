using System;
using System.Linq;

namespace name_mismatch_finder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read in data from a different project.
            var schoolsAndConferences = System.IO.File.ReadAllLines("../scrapysharp-dt2020/SchoolStatesAndConferences.csv")
                                        .Skip(1)
                                        .Where(s => s.Length > 1)
                                        .Select( s =>
                                        {
                                            var columns = s.Split(',');
                                            return new School(columns[0], columns[1], columns[2]);
                                        })
                                        .ToList();
            

            
            var ranks = System.IO.File.ReadAllLines("../scrapysharp-dt2020/ranks/2019-05-21-ranks.csv")
                                        .Skip(1)
                                        .Where(r => r.Length > 1)
                                        .Select(r =>
                                        {
                                            var columns = r.Split(',');
                                            int rank = Int32.Parse(columns[0]);
                                            string name = columns[2];
                                            string college = columns[3];
                                            string dateString = columns[8];

                                            return new ProspectRank(rank, name, college, dateString);
                                        }
                                        )
                                        .ToList();
            var schoolMismatches = from r in ranks
                                    join school in schoolsAndConferences on r.school equals school.schoolName into mm
                                    from school in mm.DefaultIfEmpty()
                                    where school is null
                                    select new {
                                        rank = r.rank,
                                        name = r.playerName,
                                        college = r.school 
                                    }
                                    ;
            
            foreach(var s in schoolMismatches){
                Console.WriteLine($"{s.rank}, {s.name}, {s.college}");
            }
            
        }
    }
    public class ProspectRank
    {
        public int rank;
        public string playerName;
        public string school;
        public string rankingDateString;

        public ProspectRank(int rank, string name, string school, string rankingDate)
        {
            this.rank = rank;
            this.playerName = name;
            this.school = school;
            this.rankingDateString = rankingDate;
        }
    }
    class School
    {
        public string schoolName;
        public string conference;
        public string state;
        public School (string schoolName, string conference, string state)
        {
            this.schoolName = schoolName;
            this.conference = conference;
            this.state = state;
        }
    }
}
