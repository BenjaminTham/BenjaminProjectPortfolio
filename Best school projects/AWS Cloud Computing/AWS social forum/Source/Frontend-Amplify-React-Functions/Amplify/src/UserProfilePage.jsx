// UserProfilePage.jsx
import React, { useEffect, useState } from "react";
import { Container, Paper, Typography, Box, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

const UserProfilePage = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState(null);

  useEffect(() => {
    const idToken = sessionStorage.getItem("idToken");
    if (idToken) {
      try {
        const base64Url = idToken.split(".")[1];
        const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
        const decoded = JSON.parse(window.atob(base64));
        setProfile(decoded);
      } catch (error) {
        console.error("Failed to decode token:", error);
      }
    }
  }, []);

  if (!profile) {
    return (
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Typography variant="h6">Loading profile...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="sm" sx={{ mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          User Profile
        </Typography>
        <Box sx={{ mt: 2 }}>
          <Typography variant="body1">
            <strong>Username:</strong> {profile["cognito:username"] || "N/A"}
          </Typography>
          <Typography variant="body1">
            <strong>Email:</strong> {profile.email || "N/A"}
          </Typography>
        </Box>
        <Box sx={{ mt: 4, textAlign: "center" }}>
          <Button variant="contained" onClick={() => navigate("/home")}>
            Back to Home
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default UserProfilePage;
