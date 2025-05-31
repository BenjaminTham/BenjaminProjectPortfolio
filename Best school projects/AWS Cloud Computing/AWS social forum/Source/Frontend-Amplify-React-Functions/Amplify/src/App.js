// App.js
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginPage from "./loginPage";      // your custom login page
import HomePage from "./pages/HomePage";    // existing home page
import TopicPage from "./pages/TopicPage";  // existing topic page
import PostPage from "./pages/PostPage";    // existing post page
import ConfirmPage from "./confirmUserPage"; // your confirmation page
import UserProfilePage from "./UserProfilePage"; // new profile page
import NavBar from "./components/NavBar";
import UserPostsPage from "./pages/UserPostsPage";

function App() {
  return (
    <Router>
      <NavBar />
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/confirm" element={<ConfirmPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/topic/:topicId" element={<TopicPage />} />
        <Route path="/post/:postId" element={<PostPage />} />
        <Route path="/profile" element={<UserProfilePage />} />
        <Route path="/user/:username" element={<UserPostsPage />} />
      </Routes>
    </Router>
  );
}

export default App;
