# CartEase API

CartEase is a RESTful API designed to manage shopping cart items for an online store. It allows users to perform CRUD operations on their shopping cart items, attach images to items, and securely authenticate using Github OAuth 2.0.

## Authentication

CartEase uses OAuth 2.0 for user authentication. Users are required to log in using their Github accounts.

### Github OAuth 2.0

To authenticate with Github:
1. Visit the login endpoint.
2. You will be redirected to Github's authentication page.
3. After successfully logging in, you will be redirected back to the application.

## Endpoints

### Cart Items

#### Get All Cart Items

- **Endpoint:** `GET /cart`
- **Description:** Get a list of all cart items for the authenticated user.
- **Response:** List of cart items with associated metadata.

#### Get Cart Item by ID

- **Endpoint:** `GET /cart/{itemId}`
- **Description:** Get details of a specific cart item.
- **Response:** Cart item details.

#### Add Cart Item

- **Endpoint:** `POST /cart`
- **Description:** Add a new item to the shopping cart.
- **Request Body:** New cart item details.
- **Response:** Newly created cart item details.

#### Update Cart Item

- **Endpoint:** `PUT /cart/{itemId}`
- **Description:** Update details of an existing cart item.
- **Request Body:** Updated cart item details.
- **Response:** Updated cart item details.

#### Delete Cart Item

- **Endpoint:** `DELETE /cart/{itemId}`
- **Description:** Delete a cart item from the shopping cart.
- **Response:** Status code indicating success.

#### Upload Item Image

- **Endpoint:** `POST /cart/{itemId}/image`
- **Description:** Upload and attach an image to a cart item.
- **Request Body:** Image file.
- **Response:** Image attachment confirmation.

## Documentation

API documentation is available using Swagger. You can access it by running the application and visiting `/swagger` in your browser.

## Model Validation

Input data is validated against predefined rules to ensure data integrity and consistency.

## Unit Testing

The application includes comprehensive unit tests to ensure the correctness of each endpoint and business logic.

## Relational Database

CartEase utilizes a relational database to store user data, cart items, and images.

## Containerization

The application can be containerized using Docker. To build and run the Docker container:
1. Build the image: 
2. Run the container: 

## HTTP Status Codes

The API responds with appropriate HTTP status codes for different scenarios to provide clear feedback to clients.

## Contributing

Contributions to CartEase are welcome! Please open an issue or pull request for any enhancements or bug fixes.

## License

CartEase is open-source and licensed under the [MIT License](LICENSE).
