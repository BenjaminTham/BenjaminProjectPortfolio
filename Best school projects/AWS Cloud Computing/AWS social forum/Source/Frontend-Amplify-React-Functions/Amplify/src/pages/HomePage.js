import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {
  Container,
  Typography,
  TextField,
  Button,
  List,
  ListItem,
  ListItemText,
  Paper,
  Box,
  Alert,
  CircularProgress,
  Fab
} from "@mui/material";
import AddIcon from '@mui/icons-material/Add';
import CloseIcon from '@mui/icons-material/Close';
import { format } from "date-fns";

function HomePage() {
  const [topics, setTopics] = useState([]);
  const [newTopic, setNewTopic] = useState({ name: "", description: "" });
  const [errorMessage, setErrorMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [showCreateTopic, setShowCreateTopic] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);

  // Fetch topics from API
  const fetchTopics = () => {
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      setErrorMessage("You must be logged in to view topics.");
      return;
    }
    setIsLoading(true);
    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getTopics", {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then((response) => {
        if (!response.ok) {
          if (response.status === 401) {
            throw new Error("Unauthorized. Please log in first to see the topics.");
          }
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        if (!Array.isArray(parsedData.data)) {
          console.error("Expected an array but got:", parsedData.data);
          return;
        }
        setTopics(parsedData.data);
        setErrorMessage("");
      })
      .catch((error) => {
        console.error("Error fetching topics:", error);
        setErrorMessage(error.message);
      })
      .finally(() => setIsLoading(false));
  };

  // Fetch posts for the left side panel (all posts sorted by most recent)
  const [allPosts, setAllPosts] = useState([]);
  useEffect(() => {
    const token = sessionStorage.getItem("idToken");
    if (!token) return;
    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getPosts", {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(response => response.json())
      .then(data => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        if (!Array.isArray(parsedData.data)) return;
        const sortedPosts = parsedData.data.sort(
          (a, b) => new Date(b.created_at) - new Date(a.created_at)
        );
        setAllPosts(sortedPosts);
      })
      .catch(error => console.error("Error fetching posts:", error));
  }, []);

  // Fetch comments for the right side panel (all comments)
  const [allComments, setAllComments] = useState([]);
  useEffect(() => {
    const token = sessionStorage.getItem("idToken");
    if (!token) return;
    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments", {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(response => response.json())
      .then(data => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        let comments = [];
        if (Array.isArray(parsedData.comments)) {
          comments = parsedData.comments;
        } else if (Array.isArray(parsedData.data)) {
          comments = parsedData.data;
        }
        setAllComments(comments);
      })
      .catch(error => console.error("Error fetching comments:", error));
  }, []);

  useEffect(() => {
    fetchTopics();
    const token = sessionStorage.getItem("idToken");
    if (token) {
      const tokenPayload = JSON.parse(atob(token.split(".")[1]));
      const isUserAdmin =
        tokenPayload["cognito:groups"]?.includes("forum-admin") || false;
      setIsAdmin(isUserAdmin);
    }
  }, []);

  const handleInputChange = (e) => {
    setNewTopic({ ...newTopic, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      setErrorMessage("You must be logged in to create topics.");
      return;
    }
    const topicWithToken = {
      ...newTopic,
      idToken: token
    };

    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/newTopic", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(topicWithToken)
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then(() => {
        setNewTopic({ name: "", description: "", idToken: "" });
        fetchTopics();
        setShowCreateTopic(false);
      })
      .catch((error) => {
        console.error("Error creating topic:", error);
        setErrorMessage("Failed to create topic. Please try again.");
      });
  };

  const filteredTopics = topics.filter((topic) =>
    topic.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <>
      {/* Left floating panel: Recent Posts */}
      <Box
        sx={{
          position: "fixed",
          left: 0,
          top: "64px", // start below navbar
          bottom: 0,
          width: "250px",
          overflowY: "auto",
          bgcolor: "background.paper",
          borderRight: "1px solid #ccc",
          p: 2,
          zIndex: 1200
        }}
      >
        <Typography variant="h6" gutterBottom>
          Recent Posts
        </Typography>
        <List>
          {allPosts.map((post) => (
            <ListItem
              key={post.post_id}
              button
              component={Link}
              to={`/post/${post.post_id}`}
              state={{ postTitle: post.name, postContent: post.content }}
            >
              <ListItemText
                primary={post.name}
                secondary={
                  post.created_at
                    ? format(new Date(post.created_at), "dd MMM yyyy, h:mm a")
                    : ""
                }
              />
            </ListItem>
          ))}
        </List>
      </Box>

      {/* Right floating panel: All Comments */}
      <Box
        sx={{
          position: "fixed",
          right: 0,
          top: "64px", // start below navbar
          bottom: 0,
          width: "250px",
          overflowY: "auto",
          bgcolor: "background.paper",
          borderLeft: "1px solid #ccc",
          p: 2,
          zIndex: 1200
        }}
      >
        <Typography variant="h6" gutterBottom>
          All Comments
        </Typography>
        <List>
          {allComments.map((comment) => (
            <ListItem key={comment.comment_id}>
              <ListItemText
                primary={comment.content}
                secondary={
                  comment.created_at
                    ? format(new Date(comment.created_at), "dd MMM yyyy, h:mm a")
                    : ""
                }
              />
            </ListItem>
          ))}
        </List>
      </Box>

      {/* Main content with margins (top margin adjusted so it is not cut off) */}
      <Container
        maxWidth="md"
        sx={{
          mt: "80px", // increased top margin to clear the navbar
          ml: "260px",
          mr: "260px",
          position: "relative"
        }}
      >
        <Typography variant="h3" gutterBottom>
          Forum Topics
        </Typography>

        {/* Search bar */}
        <Box sx={{ mb: 2, display: "flex", gap: 2 }}>
          <TextField
            label="Search by Title"
            variant="outlined"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            fullWidth
          />
        </Box>

        {errorMessage && (
          <Box sx={{ mb: 2 }}>
            <Alert severity="error">{errorMessage}</Alert>
          </Box>
        )}

        {isLoading ? (
          <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            {filteredTopics.length > 0 ? (
              <List>
                {filteredTopics.map((topic) => (
                  <Paper key={topic.topic_id} sx={{ mb: 2, p: 1 }}>
                    <ListItem
                      button
                      component={Link}
                      to={`/topic/${topic.topic_id}`}
                      state={{ topicName: topic.name }}
                    >
                      <ListItemText
                        primary={topic.name}
                        secondary={topic.description}
                      />
                    </ListItem>
                  </Paper>
                ))}
              </List>
            ) : (
              <Typography variant="body1" sx={{ mt: 2 }}>
                No topics found.
              </Typography>
            )}
          </>
        )}

        {/* Floating FAB for toggling create topic form (repositioned) */}
        {isAdmin && (
          <Box
          sx={{
            position: "fixed",
            bottom: 16,
            left: "50%",
            transform: "translateX(-50%)"
          }}
          >
            {showCreateTopic ? (
              <Button
                variant="contained"
                color="secondary"
                onClick={() => setShowCreateTopic(false)}
                startIcon={<CloseIcon />}
              >
                Close
              </Button>
            ) : (
              <Fab color="primary" onClick={() => setShowCreateTopic(true)}>
                <AddIcon />
              </Fab>
            )}
          </Box>
        )}

        {/* Floating create topic form (repositioned) */}
        {showCreateTopic && (
          <Box
            sx={{
              // position: "fixed",
              // bottom: 80,
              // right: "270px",
              // width: "300px",
              // p: 2,
              // bgcolor: "background.paper",
              // boxShadow: 3,
              // borderRadius: 2,
              // zIndex: 1000

              position: "fixed",
              bottom: 80,
              left: "50%",
              transform: "translateX(-50%)",
              width: "300px",
              p: 2,
              bgcolor: "background.paper",
              boxShadow: 3,
              borderRadius: 2,
              zIndex: 1000
            }}
          >
            <Typography variant="h6" gutterBottom>
              Create Topic
            </Typography>
            <Box
              component="form"
              onSubmit={handleSubmit}
              sx={{ display: "flex", flexDirection: "column", gap: 2 }}
            >
              <TextField
                label="Topic Name"
                name="name"
                value={newTopic.name}
                onChange={handleInputChange}
                required
              />
              <TextField
                label="Description"
                name="description"
                value={newTopic.description}
                onChange={handleInputChange}
                required
              />
              <Button variant="contained" type="submit">
                Submit
              </Button>
            </Box>
          </Box>
        )}
      </Container>
    </>
  );
}

export default HomePage;
