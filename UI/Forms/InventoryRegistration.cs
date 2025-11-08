//
// File 1: Student Registration System
// Demonstrates: OOP, inheritance, collections (List<T>), and encapsulation.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentRegistration
{
    /// <summary>
    /// Base class for a person, containing common properties.
    /// </summary>
    public abstract class Person
    {
        private static int _nextId = 1000;

        /// <summary>
        /// Gets the unique identifier for the person.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the full name of the person.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Initializes a new instance of the Person class.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        protected Person(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            Id = _nextId++;
            FirstName = firstName;
            LastName = lastName;
        }

        /// <summary>
        /// Returns a string representation of the person.
        /// </summary>
        /// <returns>The person's ID and full name.</returns>
        public override string ToString()
        {
            return $"[{Id}] {FullName}";
        }
    }

    /// <summary>
    /// Represents a Student, inheriting from Person.
    /// </summary>
    public class Student : Person
    {
        /// <summary>
        /// Gets or sets the student's major.
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// Gets the list of courses the student is enrolled in.
        /// </summary>
        public List<Course> EnrolledCourses { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Student class.
        /// </summary>
        /// <param name="firstName">The student's first name.</param>
        /// <param name="lastName">The student's last name.</param>
        /// <param name="major">The student's major.</param>
        public Student(string firstName, string lastName, string major)
            : base(firstName, lastName)
        {
            Major = major;
            EnrolledCourses = new List<Course>();
        }

        /// <summary>
        /// Enrolls the student in a course.
        /// </summary>
        /// <param name="course">The course to enroll in.</param>
        /// <returns>True if enrollment was successful, false if already enrolled.</returns>
        public bool Enroll(Course course)
        {
            if (!EnrolledCourses.Contains(course))
            {
                EnrolledCourses.Add(course);
                return true;
            }
            return false; // Already enrolled
        }

        /// <summary>
        /// Drops a course for the student.
        /// </summary>
        /// <param name="course">The course to drop.</param>
        /// <returns>True if dropping was successful, false if not enrolled.</returns>
        public bool Drop(Course course)
        {
            return EnrolledCourses.Remove(course);
        }

        /// <summary>
        /// Overrides the base ToString to include student-specific details.
        /// </summary>
        /// <returns>A string with student details.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} (Major: {Major})";
        }
    }

    /// <summary>
    /// Represents a university course.
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Gets the unique course code (e.g., "CS101").
        /// </summary>
        public string CourseCode { get; private set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the number of credits for the course.
        /// </summary>
        public int Credits { get; set; }

        /// <summary>
        /// Gets or sets the maximum capacity of the course.
        /// </summary>
        public int MaxCapacity { get; set; }

        /// <summary>
        /// Gets the list of students enrolled in this course.
        /// </summary>
        public List<Student> Roster { get; private set; }

        /// <summary>
        /// Gets the number of remaining open seats.
        /// </summary>
        public int AvailableSeats => MaxCapacity - Roster.Count;

        /// <summary>
        /// Initializes a new instance of the Course class.
        /// </summary>
        /// <param name="courseCode">The unique course code.</param>
        /// <param name="title">The course title.</param>
        /// <param name="credits">The number of credits.</param>
        /// <param name="capacity">The maximum number of students.</param>
        public Course(string courseCode, string title, int credits, int capacity)
        {
            CourseCode = courseCode;
            Title = title;
            Credits = credits;
            MaxCapacity = capacity;
            Roster = new List<Student>();
        }

        /// <summary>
        /// Adds a student to the course roster.
        /// </summary>
        /// <param name="student">The student to add.</param>
        /// <returns>True if student was added, false if already full or already on roster.</returns>
        public bool AddStudent(Student student)
        {
            if (Roster.Count >= MaxCapacity)
            {
                return false; // Course is full
            }
            if (!Roster.Contains(student))
            {
                Roster.Add(student);
                return true;
            }
            return false; // Student already on roster
        }

        /// <summary>
        /// Removes a student from the course roster.
        /// </summary>
        /// <param name="student">The student to remove.</param>
        /// <returns>True if student was removed.</returns>
        public bool RemoveStudent(Student student)
        {
            return Roster.Remove(student);
        }

        /// <summary>
        /// Returns a string representation of the course.
        /// </summary>
        /// <returns>A string with course details.</returns>
        public override string ToString()
        {
            return $"[{CourseCode}] {Title} ({Credits} Cr) - {AvailableSeats}/{MaxCapacity} seats";
        }
    }

    /// <summary>
    /// Manages the registration process, acting as a service layer.
    /// </summary>
    public class RegistrationService
    {
        private List<Student> _allStudents = new List<Student>();
        private List<Course> _allCourses = new List<Course>();

        /// <summary>
        /// Adds a new student to the system.
        /// </summary>
        public void AddStudent(Student s) => _allStudents.Add(s);

        /// <summary>
        /// Adds a new course to the system.
        /// </summary>
        public void AddCourse(Course c) => _allCourses.Add(c);

        /// <summary>
        /// Finds a student by their ID.
        /// </summary>
        /// <returns>The Student object, or null if not found.</returns>
        public Student GetStudent(int id) => _allStudents.FirstOrDefault(s => s.Id == id);

        /// <summary>
        /// Finds a course by its code.
        /// </summary>
        /// <returns>The Course object, or null if not found.</returns>
        public Course GetCourse(string code) => _allCourses.FirstOrDefault(c => c.CourseCode.Equals(code, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets all students in the system.
        /// </summary>
        public IEnumerable<Student> GetAllStudents() => _allStudents;

        /// <summary>
        /// Gets all courses in the system.
        /// </summary>
        public IEnumerable<Course> GetAllCourses() => _allCourses;

        /// <summary>
        /// Registers a student for a course.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <param name="courseCode">The code of the course.</param>
        /// <returns>A string message indicating the result.</returns>
        public string RegisterStudentForCourse(int studentId, string courseCode)
        {
            Student student = GetStudent(studentId);
            if (student == null)
            {
                return "Error: Student not found.";
            }

            Course course = GetCourse(courseCode);
            if (course == null)
            {
                return "Error: Course not found.";
            }

            if (course.AvailableSeats <= 0)
            {
                return "Error: Course is full.";
            }

            if (student.EnrolledCourses.Contains(course))
            {
                return "Error: Student is already enrolled in this course.";
            }

            // Perform two-way binding
            student.Enroll(course);
            course.AddStudent(student);

            return $"Success: {student.FullName} registered for {course.Title}.";
        }

        /// <summary>
        /// Displays the class roster for a given course.
        /// </summary>
        /// <param name="courseCode">The code of the course.</param>
        public void DisplayCourseRoster(string courseCode)
        {
            Course course = GetCourse(courseCode);
            if (course == null)
            {
                Console.WriteLine("Error: Course not found.");
                return;
            }

            Console.WriteLine($"\n--- Roster for {course.Title} [{course.CourseCode}] ---");
            if (course.Roster.Any())
            {
                foreach (var student in course.Roster)
                {
                    Console.WriteLine($" - {student.ToString()}");
                }
            }
            else
            {
                Console.WriteLine(" (No students enrolled) ");
            }
            Console.WriteLine("------------------------------------------");
        }

        /// <summary>
        /// Displays the schedule (transcript) for a given student.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        public void DisplayStudentSchedule(int studentId)
        {
            Student student = GetStudent(studentId);
            if (student == null)
            {
                Console.WriteLine("Error: Student not found.");
                return;
            }

            Console.WriteLine($"\n--- Schedule for {student.FullName} [{student.Id}] ---");
            if (student.EnrolledCourses.Any())
            {
                int totalCredits = 0;
                foreach (var course in student.EnrolledCourses)
                {
                    Console.WriteLine($" - {course.ToString()}");
                    totalCredits += course.Credits;
                }
                Console.WriteLine($"Total Credits: {totalCredits}");
            }
            else
            {
                Console.WriteLine(" (Not enrolled in any courses) ");
            }
            Console.WriteLine("-------------------------------------------");
        }
    }

    /// <summary>
    /// Main program class to run the console application.
    /// </summary>
    public class Program
    {
        private static readonly RegistrationService _service = new RegistrationService();

        public static void Main(string[] args)
        {
            Console.Title = "University Registration System";
            InitializeData();

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayAllCourses();
                        break;
                    case "2":
                        DisplayAllStudents();
                        break;
                    case "3":
                        HandleRegistration();
                        break;
                    case "4":
                        HandleViewRoster();
                        break;
                    case "5":
                        HandleViewSchedule();
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                if (!exit)
                {
                    Console.WriteLine("\nPress Enter to return to the menu...");
                    Console.ReadLine();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("  University Registration System Menu");
            Console.WriteLine("=======================================");
            Console.WriteLine("1. View All Courses");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. Register Student for Course");
            Console.WriteLine("4. View Course Roster");
            Console.WriteLine("5. View Student Schedule");
            Console.WriteLine("6. Exit");
            Console.WriteLine("---------------------------------------");
            Console.Write("Enter your choice: ");
        }

        /// <summary>
        /// Populates the service with initial data.
        /// </summary>
        private static void InitializeData()
        {
            // Add Courses
            _service.AddCourse(new Course("CS101", "Intro to Computer Science", 3, 50));
            _service.AddCourse(new Course("MATH201", "Calculus II", 4, 35));
            _service.AddCourse(new Course("ENG105", "English Composition", 3, 30));
            _service.AddCourse(new Course("PHYS210", "University Physics I", 4, 40));

            // Add Students
            _service.AddStudent(new Student("Alice", "Smith", "Computer Science"));
            _service.AddStudent(new Student("Bob", "Johnson", "Physics"));
            _service.AddStudent(new Student("Charlie", "Brown", "English"));
        }

        private static void DisplayAllCourses()
        {
            Console.WriteLine("\n--- Available Courses ---");
            foreach (var course in _service.GetAllCourses())
            {
                Console.WriteLine(course);
            }
        }

        private static void DisplayAllStudents()
        {
            Console.WriteLine("\n--- Registered Students ---");
            foreach (var student in _service.GetAllStudents())
            {
                Console.WriteLine(student);
            }
        }

        private static void HandleRegistration()
        {
            try
            {
                Console.WriteLine("\n--- Register Student ---");
                Console.Write("Enter Student ID: ");
                int studentId = int.Parse(Console.ReadLine());

                Console.Write("Enter Course Code (e.g., CS101): ");
                string courseCode = Console.ReadLine();

                string result = _service.RegisterStudentForCourse(studentId, courseCode);
                Console.WriteLine(result);
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Invalid Student ID. It must be a number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void HandleViewRoster()
        {
            Console.Write("\nEnter Course Code to view roster (e.g., CS101): ");
            string courseCode = Console.ReadLine();
            _service.DisplayCourseRoster(courseCode);
        }

        private static void HandleViewSchedule()
        {
            try
            {
                Console.Write("\nEnter Student ID to view schedule: ");
                int studentId = int.Parse(Console.ReadLine());
                _service.DisplayStudentSchedule(studentId);
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Invalid Student ID. It must be a number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
