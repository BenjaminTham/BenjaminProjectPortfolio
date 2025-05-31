import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { confirmSignUp, signIn } from "./authService"; // Import signIn
import {
  Container,
  Paper,
  Typography,
  TextField,
  Button,
  Box
} from "@mui/material";

const ConfirmUserPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { username = "", email = "", user_id} = location.state || {};

  // Use controlled inputs; now include a password for auto sign in.
  const [userEmail, setUserEmail] = useState(email);
  const [userID, setUserId] = useState(user_id);
  const [confirmationCode, setConfirmationCode] = useState("");
  const [password, setPassword] = useState(""); // New field

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      // Confirm the user
      await confirmSignUp(username, confirmationCode);

      // Auto sign in using username and password
      const session = await signIn(username, password);
      if (session && session.AccessToken) {
        sessionStorage.setItem("accessToken", session.AccessToken);

        const idToken = session.IdToken;
        let userId = null;
        
        if (idToken) {
          try {
            const tokenPayload = JSON.parse(atob(idToken.split(".")[1]));
            userId = tokenPayload.sub;

            setUserId(userId);

          } catch (error) {
            console.error("Failed to decode token:", error);
          }
        }

        // Call API to insert new user into the database
        const newUser = {
          userId: userId,
          username: username,
          email: userEmail,
        };

        console.log(newUser);

        const response = await fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/newUser", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${idToken}`
          },
          body: JSON.stringify(newUser)
        });

        console.log(response);

        if (!response.ok) {
          throw new Error(`Failed to insert user: ${response.status}`);
        }

        navigate("/home");
      } else {
        alert("Sign in failed after confirmation.");
      }
    } catch (error) {
      alert(`Failed to confirm account: ${error}`);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Confirm Account
        </Typography>
        <Typography variant="body1" align="center" sx={{ mb: 2 }}>
          Please enter the confirmation code sent to your email and your password.
        </Typography>
        <Box
          component="form"
          onSubmit={handleSubmit}
          sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
          <TextField
            label="Email"
            type="email"
            value={userEmail}
            onChange={(e) => setUserEmail(e.target.value)}
            required
          />
          <TextField
            label="Confirmation Code"
            type="text"
            value={confirmationCode}
            onChange={(e) => setConfirmationCode(e.target.value)}
            required
          />
          <TextField
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <Button variant="contained" type="submit" size="large">
            Confirm & Sign In
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default ConfirmUserPage;
