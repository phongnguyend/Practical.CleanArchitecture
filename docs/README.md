### 1. What is automated testing?

Automated testing is the practice of writing code to test our code and then run those tests in an automated fashion. So, with automated testing, our source code consists of application code, which we also call production code and test code.

### 2. Benefits of Automated Testing.

- Test your code frequently in less time.
- Catch bugs before deploying.
- Deploy with confidence.
- Refactor with confidence.

Refactoring means changing the structure of the code without changing its behavior.
Ex: if you extract a few lines of a method into a separate private method, that's refactoring. If you rename a method, that's refactoring too. You've changing the structure of your code to make it cleaner and more maintainable, but you're not changing the functionality.

- Focus more on the quality.

### 3. Some types of Tests

- Unit test
- Integration test.
- End-to-end test.

### 4. What is unit test?

Tests a unit of an application without its external dependencies such as files, databases, message queues, web services and so on.
Cheap to write and execute fast.

### 5. What is integration test?

Tests the application with its external dependencies. So it tests the integration of your application code with these concrete dependencies like files, databases, and so on.
Take longer to execute.
Give more confidence.

### 6. What is end-to-end test?

Drives an application through its UI.
Very slow.
Very brittle, because a small enhancement to the application or small change in the UI can easily break these tests.

### 7. What is a unit test framework?

To write unit and integration tests, you need a testing framework. It give you a utility library to write your tests and a test runner which runs your tests and gives you a report of passing and failing tests.

### 8. What is test driven development (TDD)?

TDD also called test first is an approach to build software. With TDD you write yours tests before writing the production code.

### 9. How TDD works?

You start by writing a failing test. This test should fail because you don't have any application code what would make it pass. Then you will write the simplest application code that will make this test pass. Then refactor your code if necessary. You repeat these three steps over and over until you build a complete feature.

### 10. What are benefits of TDD?

- Your source code will be testable right from the start.
- Every line of your production code is fully covered by tests. Which means you can refactor and deploy with confidence.

### 11. Characteristics of Good Unit Tests

- Each test should have a single responsibility and should have few lines of code.
- Should not have any logic, It means you should not have any conditional statements, loops and other kinds of logic in your tests. Simply call a method and make an assertion.
- Each test should be written and executed in an isolated mode. So your tests methods should not call each other and they should not assume any state created by another test.
- Should not be too specific or too general.

### 12. What to test and what not to test?

Test the outcome of a method. We have 2 types of function: queries and commands.
For testing a query function, you should write a test and verify that your function is returning the right value. You might have multiple execution paths in that function. In that case, you should test all the execution paths and make sure that each execution path results in the right value.
For command methods, you should test the outcome of this method. If the outcome is to change the state of an object in memory, you should test that the given object is now in the right state. If the outcome is to talk to an external like a database or web service, you should verify that the class on the test is making the right call to these external dependencies.

You should never test language features and third party code. For example C# language features, entity framework. You should only test your code.

### 13. Best practice to name and organize tests

For each project in our solution we will have a unit testing project.
Should separate unit and integration tests. Because unit test execute fast, integration test take longer and we want to run unit tests frequently as we are writing code and run integration tests just before committing our code to the repository to make sure everything works.
In unit test project, we have a test class for each class in our production code. If we have a class UserService we should have a class called UserServiceTests.
For each method in the test class we should have one or more test methods.
The name of test method should follow this convention: MethodName_Scenario_ExpectedBehavior

### 14. How many test should you write for a specific method?

The number of tests is often equal to or greater than the number of execution paths.

### 15. What is a trustworthy test?

A trustworthy test is the kind of test we can rely on. So if the test passes, you know that your code is working, and if it fails, you know that there is something wrong with your code.
If there is a bug in the production code and your test is still passing, it is not a trustworthy test because it tests the wrong thing.
One way to write trustworthy test is using TDD.

### 16. How do we know if we have enough tests for a method or a class?

Using a code coverage tool.

### 17. What is code coverage tool?

A tool to scan your code and tells you what parts of your code are not tested.

### 18. How can we unit test a class that depends on an external resource?

Unit test should not touch external resources. A test that touches an external resource is classified as an integration test. But we can still unit test the logic in our classes without touching their external dependencies.
When unit test classes with external dependencies, we replace a production object with a fake object. But this requires we have to decouple our classes from their external resources.

### 19. What is testable code?

That code has to be loosely coupled with it dependencies which talk to external resources. In a loosely coupled design we can replace one object with another at run time. So when unit testing a class that uses an object that talks to an external resource, we can replace that object with a fake object. That means the code should only depends on abstraction or interface or contract, not depends on any specific implementation.

When we program against interfaces, we can provide different implementations at different times. In our production code, we will provide the real implementation that talks to an external resource. in our test we provide a fake implementation and this is what we call dependency injection. instead of newing up dependencies, we inject them from the outside.

### 20. What is dependency injection?

Injecting or supplying the dependencies of a class from the outside and this makes our classes loosely coupled and testable.

### 21. What is a dependency framework?

A dependency framework will take care of creating and initializing objects at run time.
In a dependency framework we have a container, this container is a registry for all our interfaces and their implementations. When our application starts, the dependency injection framework will automatically take care of creating object graphs based on the interfaces and types registered in the container.

### 22. What is mocking framework?

Creating each fake object for each testing scenario by hand takes a lot of time. So that is why we use a mocking or isolation framework. It helps us dynamically create fake or mock objects. So we don't have to code them by hand. We can create them dynamically as part of our tests and program them to behave any way we want. So we can program them to throw exceptions, to return values, to raise events and so on.

### 23. What is interaction testing?

Sometimes We deal with code that touches external resources, we need to verify the class we are testing interacts with another class properly, this is what we call  interaction testing. Because we test the interaction of one class with another. We verify that the right method is called with the right arguments
Use interaction testing only when dealing with external resources.

### 24. Who should write unit tests?

Writing unit and integration tests is the job of a software developer. So if you building a new feature, modifying an existing feature or fixing a bug, before committing your code to the repository, you are responsible to cover your code with a suit of unit and integration tests. You cannot expect someone else to write the tests for the code that you have written. This is exactly like writting a bit of code that doesn't compile and then passing it to your coworker asking them to fix the compilation issue.
