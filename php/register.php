<?php
require_once 'db_config.php';

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

$name = $_POST['name'];
$email = $_POST['email'];
$password = $_POST['password'];

// Validar si el nombre ya existe
$stmtName = $conn->prepare("SELECT id FROM users WHERE name = ?");
$stmtName->bind_param("s", $name);
$stmtName->execute();
$stmtName->store_result();

if ($stmtName->num_rows > 0) {
    echo json_encode(["success" => false, "message" => "Este nombre de usuario ya está en uso", "code" => "existing_name"]);
    $stmtName->close();
    $conn->close();
    exit;
}
$stmtName->close();

// Validar si el email ya existe
$stmtEmail = $conn->prepare("SELECT id FROM users WHERE email = ?");
$stmtEmail->bind_param("s", $email);
$stmtEmail->execute();
$stmtEmail->store_result();

if ($stmtEmail->num_rows > 0) {
    echo json_encode(["success" => false, "message" => "Este email ya está en uso", "code" => "existing_email"]);
    $stmtEmail->close();
    $conn->close();
    exit;
}
$stmtEmail->close();

// Registrar al usuario

// Crear entrada por defecto en customization
$conn->query("INSERT INTO customization (spaceship_color, propulsion_color, trail_color, shot_color, spaceship_skin, propulsion_skin, trail_skin, shot_skin) VALUES (DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)");
$idCustomization = $conn->insert_id;

// Crear entrada por defecto en settings
$conn->query("INSERT INTO settings (volume_main, volume_music, volume_fx, size_joystick, size_buttons, language) VALUES (DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)");
$idSetting = $conn->insert_id;

// Registrar al usuario con los IDs obtenidos
$hashedPassword = password_hash($password, PASSWORD_DEFAULT);
$stmt = $conn->prepare("INSERT INTO users (name, email, password, id_customization, id_settings) VALUES (?, ?, ?, ?, ?)");
$stmt->bind_param("sssii", $name, $email, $hashedPassword, $idCustomization, $idSetting);

if ($stmt->execute()) {
    echo json_encode(["success" => true, "message" => "Usuario registrado correctamente", "code" => "register_successful"]);
} else {
    echo json_encode(["success" => false, "message" => "Error al registrar usuario", "code" => "register_error"]);
}

$stmt->close();
$conn->close();
?>