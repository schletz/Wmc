<?php
require_once("controller.class.php");
class UserController extends Controller
{
    // Default Route (Request mit Parameter controller ohne method)
    // GET /api/?controller=user
    public function get()
    {
        $this->sendNotFoundAndExit();
    }

    /**
     * Adds a new user to the database.
     * POST /api/?controller=user&method=addUser
     * Creates a user.
     * Expected payload (JSON):
     *     {username: string, password: string, firstname: string, lastname: string, email: string}
     * @return void
     */
    public function addUser()
    {
        $hashedPass = password_hash($this->requestBody->password, PASSWORD_DEFAULT);
        $guid = $this->generateGuid();
        $this->getData(
            "INSERT INTO User (Guid, Username, PasswordHash, Firstname, Lastname, Email) VALUES(?, ?, ?, ?, ?, ?)",
            array(
                $guid, $this->requestBody->username, $hashedPass,
                $this->requestBody->firstname, $this->requestBody->lastname, $this->requestBody->email
            )
        );
        // 204 no content an den Client senden.
        $this->sendNoContentAndExit();
    }


    /**
     * Gets the encoded data in the token or cookie.
     * Requires authenticated user.
     * GET /api?controller=user&method=getProfile
     */
    public function getProfile()
    {
        $cookie = $this->checkAuthentication();
        return json_encode($cookie);
    }

    /**
     * login
     * POST /api/?controller=user&method=login
     * Fordert ein Authentication Cookie an. Wird in der Angular App verwendet.
     * @return void
     */
    public function login()
    {
        $username = $this->requestBody->username;
        $password = $this->requestBody->password;
        $result = $this->getData(
            "SELECT Username, PasswordHash, Firstname, Lastname, Email FROM User WHERE Username = ?",
            array($username),
            asJson: false
        );
        if (count($result) != 1) {
            $this->sendUnauthorizedAndExit();
        }
        if (!password_verify($password, $result[0]["PasswordHash"])) {
            $this->sendUnauthorizedAndExit();
        }
        $cookieData = [
            "user" => $username, "firstname" => $result[0]["Firstname"],
            "lastname" => $result[0]["Lastname"], "email" => $result[0]["Email"], "role" => 0
        ];
        $this->setCookieHeader($cookieData);
        return json_encode($cookieData);
    }
    
    /**
     * logout
     * Sendet ein abgelaufenes, leeres Cookie um es zu löschen.
     * @return void
     */
    public function logout()
    {
        $this->setDeleteCookieHeader();
        $this->sendNoContentAndExit();
    }
    
    /**
     * getAuthToken
     * Fordert einen Auth Token an. Wird für die Android App verwendet. Der Token wird dann als
     * Bearer Token im Authorize Header an die API geschickt.
     * @return void
     */
    public function getAuthToken()
    {
        $username = $this->requestBody->username;
        $password = $this->requestBody->password;
        $result = $this->getData(
            "SELECT Username, PasswordHash, Firstname, Lastname, Email FROM User WHERE Username = ?",
            array($username),
            asJson: false
        );
        if (count($result) != 1) {
            $this->sendUnauthorizedAndExit();
        }
        if (!password_verify($password, $result[0]["PasswordHash"])) {
            $this->sendUnauthorizedAndExit();
        }
        $token = $this->generateAuthToken([
            "user" => $username, "firstname" => $result[0]["Firstname"],
            "lastname" => $result[0]["Lastname"], "email" => $result[0]["Email"], "role" => 0
        ], time() + 60 * 60 * 3);
        return json_encode(["authToken" => $token]);
    }
}
