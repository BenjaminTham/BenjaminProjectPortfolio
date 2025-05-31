// LoginPage.jsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { signIn, signUp } from "./authService";

// --- MUI Imports ---
import {
  Container,
  Typography,
  TextField,
  Button,
  Box,
  Paper
} from "@mui/material";

const LoginPage = () => {
  const [username, setUsername] = useState(""); 
  const [email, setEmail] = useState("");      
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isSignUp, setIsSignUp] = useState(false);
  const navigate = useNavigate();

  // Sign in with username + password
  const handleSignIn = async (e) => {
    e.preventDefault();
    try {
      const session = await signIn(username, password);
      if (session && typeof session.AccessToken !== "undefined") {
        sessionStorage.setItem("accessToken", session.AccessToken);
        // Optionally store idToken, refreshToken as well
        if (sessionStorage.getItem("accessToken")) {
          // Navigate to /home or do window.location.href = "/home"
          navigate("/home");
        } else {
          console.error("Session token was not set properly.");
        }
      } else {
        console.error("SignIn session or AccessToken is undefined.");
      }
    } catch (error) {
      alert(`Sign in failed: ${error}`);
    }
  };

  // Sign up with username, email, password, confirm password
  const handleSignUp = async (e) => {
    e.preventDefault();
    if (password !== confirmPassword) {
      alert("Passwords do not match");
      return;
    }
    try {
      await signUp(username, email, password);
      // If your pool requires confirmation, navigate to /confirm or show success
      // navigate("/confirm");
      navigate("/confirm", { state: { username, email } });

    } catch (error) {
      alert(`Sign up failed: ${error}`);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 6 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Welcome
        </Typography>
        <Typography variant="subtitle1" align="center" paragraph>
          {isSignUp
            ? "Sign up to create an account"
            : "Sign in to your account"}
        </Typography>

        <Box
          component="form"
          onSubmit={isSignUp ? handleSignUp : handleSignIn}
          sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
          {/* Username always shown; if your pool is email-based, adapt accordingly */}
          <TextField
            id="username"
            label="Username"
            variant="outlined"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />

          {/* Email only if signing up */}
          {isSignUp && (
            <TextField
              id="email"
              label="Email"
              type="email"
              variant="outlined"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          )}

          {/* Password */}
          <TextField
            id="password"
            label="Password"
            type="password"
            variant="outlined"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          {/* Confirm password if signing up */}
          {isSignUp && (
            <TextField
              id="confirmPassword"
              label="Confirm Password"
              type="password"
              variant="outlined"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          )}

          <Button variant="contained" type="submit" size="large">
            {isSignUp ? "Sign Up" : "Sign In"}
          </Button>
        </Box>

        <Box mt={2} textAlign="center">
          <Button
            variant="text"
            onClick={() => setIsSignUp(!isSignUp)}
          >
            {isSignUp
              ? "Already have an account? Sign In"
              : "Need an account? Sign Up"}
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default LoginPage;
