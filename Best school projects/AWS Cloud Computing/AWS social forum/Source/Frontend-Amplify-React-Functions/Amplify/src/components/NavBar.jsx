import React from "react";
import { AppBar, Toolbar, Typography, Button } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";

function NavBar() {
  const navigate = useNavigate();
  const idToken = sessionStorage.getItem("idToken");
  let decodedPayload = {};
  if (idToken) {
    try {
      const base64Url = idToken.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      decodedPayload = JSON.parse(window.atob(base64));
    } catch (err) {
      console.error("Error decoding id token:", err);
    }
  }
  const usernameOrEmail =
    decodedPayload["cognito:username"] || decodedPayload.email || "Unknown";

  const handleSignOut = () => {
    sessionStorage.removeItem("idToken");
    sessionStorage.removeItem("accessToken");
    sessionStorage.removeItem("refreshToken");
    navigate("/");
  };

  return (
    <AppBar position="fixed" sx={{ zIndex: 1300 }}>
      <Toolbar>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          <Link to="/home" style={{ color: "inherit", textDecoration: "none" }}>
          Oh My Gaviscon!
          </Link>
        </Typography>
        {idToken ? (
          <>
            <Button color="inherit" onClick={() => navigate("/profile")}>
              <Typography variant="body1" sx={{ mr: 2 }}>
                {usernameOrEmail}
              </Typography>
            </Button>
            <Button color="inherit" onClick={handleSignOut}>
              Sign Out
            </Button>
          </>
        ) : (
          <Button color="inherit" onClick={() => navigate("/")}>
            Sign In
          </Button>
        )}
      </Toolbar>
    </AppBar>
  );
}

export default NavBar;
