namespace _01.SchoolCompetition
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class SchoolCompetition
    {
        public static void Main()
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();
            Dictionary<string, SortedSet<string>> categories = new Dictionary<string, SortedSet<string>>();

            string line = Console.ReadLine();
            while (line.ToLower() != "end")
            {
                string[] args = line.Split();
                string name = args[0];
                string category = args[1];
                int score = int.Parse(args[2]);
                if (!scores.ContainsKey(name))
                {
                    scores.Add(name, 0);
                    
                }

                scores[name] += score;

                if (!categories.ContainsKey(name))
                { 
                    categories[name] = new SortedSet<string>();
                }

                categories[name].Add(category);

                line = Console.ReadLine();
            }

            var students = scores
                .OrderByDescending(p => p.Value)
                .Select(p => p.Key);

            foreach (var student in students)
            {
                string strCategories = $"[{string.Join(", ", categories[student])}]";
                Console.WriteLine($"{student}: {scores[student]} {strCategories}");
            }
        }
    }
}