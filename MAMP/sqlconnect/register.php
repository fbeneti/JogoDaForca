<?php

    // ENDEREÇO, USUARIO, PASSWORD, NOME DO BANCO DE DADOS
    $con = mysqli_connect('localhost', 'root', 'root', 'jogodavelha');

    //CHECK THAT CONNECTION HAPPENED
    if (mysqli_connect_errno())
    {
        echo "1: Connection failed" //ERROR CODE #1 - Connection failed
        exit();
    }

    $email = $_POST["email"];
    $username = $_POST["username"];
    $password = $_POST["password"];

    //CHECK IF EMAIL EXISTS
    $emailcheckquery = "SELECT email FROM players WHERE email='" . $email . "';";
    $emailcheck = mysqli_query($con, $emailcheckquery) or die("2: E-mail check query failed"); //ERROR CODE #2 - E-mail check query failed
    if (mysqli_num_rows($emailcheck) > 0)
    {
        echo "3: E-mail already exists"; // ERROR CODE #3 - E-mail already exists
        exit();
    }    

    //CHECK IF USERNAME EXISTS
    $usernamecheckquery = "SELECT username FROM players WHERE username='" . $username . "';";
    $usernamecheck = mysqli_query($con, $usernamecheckquery) or die("4: Username check query failed"); //ERROR CODE #4 - Username check query failed
    if (mysqli_num_rows($usernamecheck) > 0)
    {
        echo "5: Username already exists"; // ERROR CODE #5 - Username already exists
        exit();
    }    

    //ADD USER TO THE TABLE
    $salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
    $hash = crypt($password, $salt);
    $insertuserquery = "INSERT INTO players (email, username, hash, salt) VALUES ('" . $email . "', '" . $username . "', '" . $hash . "', '" . $salt . "');";
    mysqli_query($con, $insertuserquery) or die("6: Insert query failed"); //ERROR CODE #6 - Insert query failed

    echo("0"); 
?>
