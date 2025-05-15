<?php
require_once 'db_config.php';

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

if (!isset($_POST['session_token']) || !isset($_POST['field']) || !isset($_POST['value'])) {
    echo json_encode(["success" => false, "message" => "Faltan los parametros", "code" => "parameters_not_found"]);
    exit();
}

$session_token = $_POST['session_token'];
$field = $_POST['field'];
$value = $_POST['value'];

// Lista de campos permitidos y tipo esperado
$allowed_fields = [
    "spaceship_color" => "s",
    "propulsion_color" => "s",
    "trail_color" => "s",
    "shot_color" => "s",
    "spaceship_skin" => "i",
    "propulsion_skin" => "i",
    "trail_skin" => "i",
    "shot_skin" => "i"
];

if (!array_key_exists($field, $allowed_fields)) {
    echo json_encode(["success" => false, "message" => "Campo invalido - '$field'", "code" => "field_invalid"]);
    exit();
}

// Obtener tipo esperado
$param_type = $allowed_fields[$field];

// Validar tipo
$validators = [
    "i" => fn($v) => ctype_digit($v), //int
    "s" => fn($v) => is_string($v), //string
    "d" => fn($v) => is_numeric($v), //double
    "b" => fn($v) => $v === "0" || $v === "1" //bool
];

if (!isset($validators[$param_type]) || !$validators[$param_type]($value)) {
    echo json_encode(["success" => false, "message" => "Valor invalido para campo de tipo '$param_type'", "code" => "value_invalid"]);
    exit();
}

$queryUser = $conn->prepare("SELECT id_customization FROM users WHERE session_token = ?"); // Busca el id_customization del usuario
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

    $queryString = "UPDATE customization SET $field = ? WHERE id = ?";
    $queryUpdate = $conn->prepare($queryString);
    $queryUpdate->bind_param($param_type . "i", $value, $id_customization);

    if ($queryUpdate->execute()) {
        echo json_encode(["success" => true, "message" => "Campo actualizado correctamente", "code" => "update_field_successful"]);
    } else {
        echo json_encode(["success" => false, "message" => "Fallo al actualizar el campo", "code" => "update_field_error"]);
    }

    $queryUpdate->close();
} else {
    echo json_encode(["success" => false, "message" => "Token de sesion invalido", "code" => "session_token_invalid"]);
}

$queryUser->close();
$conn->close();
?>