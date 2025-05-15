<?php
require_once 'db_config.php';

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

if (!isset($_POST['session_token'])) {
    echo json_encode(["success" => false, "message" => "Falta el token de sesion", "code" => "session_token_not_found"]);
    exit();
}

$session_token = $_POST['session_token'];

$queryUser = $conn->prepare("SELECT id_customization FROM users WHERE session_token = ?");
$queryUser->bind_param("s", $session_token);
$queryUser->execute();
$resultUser = $queryUser->get_result();

if ($resultUser->num_rows > 0) {
    $userData = $resultUser->fetch_assoc();
    $id_customization = $userData['id_customization'];

    if ($id_customization == null) {
        echo json_encode(["success" => false, "message" => "No se ha encontrado usuario con esta personalizacion", "code" => "user_not_found"]);
        exit();
    }

    $queryCustomization = $conn->prepare("SELECT spaceship_color, propulsion_color, trail_color, shot_color, spaceship_skin, propulsion_skin, trail_skin, shot_skin FROM customization WHERE id = ?");
    $queryCustomization->bind_param("i", $id_customization);
    $queryCustomization->execute();
    $resultCustomization = $queryCustomization->get_result();

    if ($resultCustomization->num_rows > 0) {
        $customization = $resultCustomization->fetch_assoc();
        echo json_encode([
            "success" => true,
            "customization" => $customization
        ]);
    } else {
        echo json_encode(["success" => false, "message" => "No se ha encontrado ajustes de personalizacion", "code" => "customization_not_found"]);
    }

    $queryCustomization->close();
} else {
    echo json_encode(["success" => false, "message" => "Token de sesion invalido", "code" => "session_token_invalid"]);
}

$queryUser->close();
$conn->close();
?>