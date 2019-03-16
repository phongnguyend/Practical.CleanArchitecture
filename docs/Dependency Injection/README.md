### 1. What is dependency injection?

Injecting or supplying the dependencies of a class from the outside and this makes our classes loosely coupled and testable.

### 2. What is a dependency framework?

A dependency framework will take care of creating and initializing objects at run time.
In a dependency framework we have a container, this container is a registry for all our interfaces and their implementations. When our application starts, the dependency injection framework will automatically take care of creating object graphs based on the interfaces and types registered in the container.
