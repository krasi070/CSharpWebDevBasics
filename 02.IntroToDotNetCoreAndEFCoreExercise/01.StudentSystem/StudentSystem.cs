namespace _01.StudentSystem
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Data;

    public class StudentSystem
    {
        private static Random random = new Random();

        public static void Main()
        {
            using (var db = new StudentSystemDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                PopulateDb(db);
            }
        }

        private static void PrintActiveCourses(StudentSystemDbContext db, DateTime date)
        {
            var courses = db.Courses
                .Where(c => c.StartDate.CompareTo(date) < 0 && c.EndDate.CompareTo(date) > 0)
                .Select(c => new
                {
                    c.Name,
                    c.StartDate,
                    c.EndDate,
                    CourseDuration = c.EndDate.Subtract(c.StartDate),
                    NumberOfEnrolledStudents = c.Students.Count
                })
                .OrderByDescending(c => c.NumberOfEnrolledStudents)
                .ThenByDescending(c => c.CourseDuration);

            foreach (var course in courses)
            {
                Console.WriteLine(course.Name);
                Console.WriteLine($"Start Date: {course.StartDate.Day}/{course.StartDate.Month}/{course.StartDate.Year}");
                Console.WriteLine($"End Date: {course.EndDate.Day}/{course.EndDate.Month}/{course.EndDate.Year}");
                Console.WriteLine($"Duration: {course.CourseDuration.TotalDays} days");
                Console.WriteLine($"Enrolled Students: {course.NumberOfEnrolledStudents}");
                Console.WriteLine();
            }
        }

        private static void PrintStudentCourseData(StudentSystemDbContext db)
        {
            var students = db.Students
                .Where(s => s.Courses.Count > 0)
                .Select(s => new
                {
                    s.Name,
                    CoursesCount = s.Courses.Count,
                    TotalPrice = s.Courses.Sum(sc => sc.Course.Price),
                    AveragePrice = s.Courses.Average(sc => sc.Course.Price)
                })
                .OrderByDescending(s => s.TotalPrice)
                .ThenByDescending(s => s.CoursesCount)
                .ThenBy(s => s.Name);

            foreach (var student in students)
            {
                Console.WriteLine($"{student.Name}");
                Console.WriteLine($"Number of Courses: {student.CoursesCount}");
                Console.WriteLine($"Total Price: {student.TotalPrice:F2}");
                Console.WriteLine($"Average Price: {student.AveragePrice:F2}");
                Console.WriteLine();
            }
        }

        private static void PrintStudentsAndHomeworkSubmissions(StudentSystemDbContext db)
        {
            var students = db.Students
                .Select(s => new
                {
                    s.Name,
                    HomeworkSubmissions = s.HomeworkSubmissions
                    .Select(h => new
                    {
                        h.Content,
                        h.ContentType
                    })
                });

            foreach (var student in students)
            {
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Name: {student.Name}");
                Console.WriteLine(new string('-', 50));
                foreach (var hwSubmission in student.HomeworkSubmissions)
                {
                    Console.WriteLine();
                    Console.WriteLine($"--Content: {hwSubmission.Content}");
                    Console.WriteLine($"--Type: {hwSubmission.ContentType}");
                }
            }
        }

        private static void PrintCoursesWithMoreThanFiveResources(StudentSystemDbContext db)
        {
            var courses = db.Courses
                .Where(c => c.Resources.Count > 5)
                .OrderByDescending(c => c.Resources.Count)
                .ThenByDescending(c => c.StartDate)
                .Select(c => new
                {
                    c.Name,
                    ResourceCount = c.Resources.Count
                });

            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Name}: {course.ResourceCount} resources");
            }
        }

        private static void PrintCoursesWithResources(StudentSystemDbContext db)
        {
            var courses = db.Courses
                .OrderBy(c => c.StartDate)
                .ThenByDescending(c => c.EndDate)
                .Select(c => new
                {
                    c.Name,
                    c.Description,
                    Resources = c.Resources
                    .Select(r => new
                    {
                        r.Name,
                        r.ResourceType,
                        r.Url
                    })
                });

            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Name} ({course.Description})");
                foreach (var resource in course.Resources)
                {
                    Console.WriteLine($"--{resource.Name};{resource.ResourceType.ToString()} ({resource.Url})");
                }

                Console.WriteLine(new string('-', 50));
            }
        }

        private static void PrintCoursesWithResourcesAndLicenses(StudentSystemDbContext db)
        {
            var courses = db.Courses
                .Select(c => new
                {
                    c.Name,
                    Resources = c.Resources
                    .Select(r => new
                    {
                        r.Name,
                        Licenses = r.Licenses
                        .Select(l => new
                        {
                            l.Name
                        })
                    })
                    .OrderByDescending(r => r.Licenses.Count())
                    .ThenBy(r => r.Name)
                })
                .OrderByDescending(c => c.Resources.Count())
                .ThenBy(c => c.Name);

            foreach (var course in courses)
            {
                Console.WriteLine(course.Name);
                foreach (var resource in course.Resources)
                {
                    Console.WriteLine($"--{resource.Name}");
                    foreach (var license in resource.Licenses)
                    {
                        Console.WriteLine($"----{license.Name}");
                    }
                }

                Console.WriteLine();
            }
        }

        private static void PopulateDb(StudentSystemDbContext db)
        {
            AddStudents(db);
            AddCourses(db);
            AddStudentsToCourses(db);
            AddResources(db);
            AddHomeworkSubmissions(db);
            AddLicenses(db);
        }

        private static void AddStudents(StudentSystemDbContext db, int numberOfStudents = 25)
        {
            for (int i = 0; i < numberOfStudents; i++)
            {
                Student newStudent = new Student()
                {
                    Name = $"Student No. {i}",
                    PhoneNumber = $"Phone Number No. {i}",
                    RegistrationDate = DateTime.Now.AddDays(-i),
                    Birthday = DateTime.Now.AddYears(-20).AddDays(i),
                };

                db.Students.Add(newStudent);
            }

            db.SaveChanges();
        }

        private static void AddCourses(StudentSystemDbContext db, int numberOfCourses = 10)
        {
            for (int i = 0; i < numberOfCourses; i++)
            {
                DateTime startDate = DateTime.Now.AddYears(-i % 2).AddMonths(i % 5).AddDays(i);
                Course newCourse = new Course()
                {
                    Name = $"Course No. {i}",
                    Description = i.ToString(),
                    StartDate = startDate,
                    EndDate = startDate.AddMonths(2),
                    Price = (i % 3 + 1) * 100
                };

                db.Courses.Add(newCourse);
            }

            db.SaveChanges();
        }

        private static void AddStudentsToCourses(StudentSystemDbContext db)
        {
            var studentIds = db.Students
                .Select(s => s.Id)
                .ToList();
            var courseIds = db.Courses
                .Select(c => c.Id)
                .ToList();
            for (int i = 0; i < courseIds.Count; i++)
            {
                int studentsInCourse = random.Next(2, studentIds.Count / 2);
                bool[] used = new bool[studentIds.Count];
                for (int j = 0; j < studentsInCourse; j++)
                {
                    int index = random.Next(0, studentIds.Count);
                    if (used[index])
                    {
                        continue;
                    }

                    used[index] = true;
                    int studentId = studentIds[index];
                    db.Courses
                        .FirstOrDefault(c => c.Id == courseIds[i])
                        .Students
                        .Add(new StudentCourse()
                        {
                            StudentId = studentId,
                            CourseId = courseIds[i]
                        });
                }
            }

            db.SaveChanges();
        }

        private static void AddResources(StudentSystemDbContext db, int numberOfResources = 100)
        {
            var courseIds = db.Courses
                .Select(c => c.Id)
                .ToList();
            int resourcesPerCourse = numberOfResources / courseIds.Count;
            int enumCount = Enum.GetValues(typeof(ResourceType)).Length;
            for (int i = 0; i < courseIds.Count; i++)
            {
                for (int j = 0; j < resourcesPerCourse; j++)
                {
                    Resource newResource = new Resource()
                    {
                        Name = $"Resource {j} for Course {i}",
                        ResourceType = (ResourceType)random.Next(0, enumCount),
                        Url = $"Fake Url {i}{j}",
                        CourseId = courseIds[i]
                    };

                    db.Resources.Add(newResource);
                }
            }

            db.SaveChanges();
        }

        private static void AddHomeworkSubmissions(StudentSystemDbContext db)
        {
            var studentIds = db.Students.Select(s => s.Id).ToList();
            int enumCount = Enum.GetValues(typeof(ResourceType)).Length;
            for (int i = 0; i < studentIds.Count; i++)
            {
                var courses = db.Students
                    .FirstOrDefault(s => s.Id == studentIds[i])
                    .Courses
                    .Select(c => c.CourseId)
                    .ToList();    
                for (int j = 0; j < courses.Count; j++)
                {
                    Homework newHomework = new Homework()
                    {
                        Content = $"By Student No. {studentIds[i]} for Course {courses[j]}",
                        ContentType = (ContentType)random.Next(0, enumCount),
                        SubmissionDate = DateTime.Now.AddDays(i - j),
                        CourseId = courses[j],
                        StudentId = studentIds[i]
                    };

                    db.HomeworkSubmissions.Add(newHomework);
                }
            }

            db.SaveChanges();
        }

        private static void AddLicenses(StudentSystemDbContext db)
        {
            var resourceIds = db.Resources
                .Select(r => r.Id)
                .ToList();

            for (int i = 0; i < resourceIds.Count; i++)
            {
                int numberOfLicenses = random.Next(1, 4);
                for (int j = 0; j < numberOfLicenses; j++)
                {
                    License newLicense = new License()
                    {
                        Name = $"License {i}{j}",
                        ResourceId = resourceIds[i]
                    };

                    db.Licenses.Add(newLicense);
                }
            }

            db.SaveChanges();
        }
    }
}