import React, { useState, useEffect } from "react";
import { useLocation, useParams, Link } from "react-router-dom";
import { format } from "date-fns";
import {
  Container,
  Typography,
  TextField,
  Button,
  List,
  Paper,
  Box,
  CircularProgress,
  Fab,
  ListItem,
  ListItemText,
  MenuItem
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import CloseIcon from "@mui/icons-material/Close";

const TopicPage = () => {
  const { topicId } = useParams();
  const location = useLocation();
  const topicName = location.state?.topicName || "Unknown Topic";

  const [posts, setPosts] = useState([]);
  const [newPost, setNewPost] = useState({ name: "", content: "" });
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showCreatePost, setShowCreatePost] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [sortOrder, setSortOrder] = useState("newest");
  const [selectedFile, setSelectedFile] = useState(null);
  const [imageURLs, setImageURLs] = useState({});
  const [filePreview, setFilePreview] = useState(null);
  const [getfileType, setfileType] = useState("");

  // Function to fetch an image from S3
  const fetchImageFromS3 = async (postID) => {
    const new_postID = topicId + "_" + postID;
    try {
      const response = await fetch(
        "https://h2ngxg46k3.execute-api.ap-southeast-2.amazonaws.com/test-stage/retrieve-s3",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ partial_name: new_postID })
        }
      );
      if (response.status === 404) return null;
      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);
      const data = await response.json();
      const parsedData = data.body ? JSON.parse(data.body) : data;
      return parsedData.file_content || null;
    } catch (error) {
      console.error("Error fetching image:", error);
      return null;
    }
  };

  // Function to store an image to S3
  const storeImageToS3 = (newPostId) => {
    const fileExtension = getfileType.split("/").pop().toLowerCase();
    const new_filename = `${topicId}_${newPostId}.${fileExtension}`;
    const base64Content = filePreview.split(",")[1];
    const payload = {
      content: base64Content,
      file_name: new_filename
    };
    return fetch(
      "https://h2ngxg46k3.execute-api.ap-southeast-2.amazonaws.com/test-stage/store-s3",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      }
    )
      .then((response) => response.json())
      .then((data) => {
        console.log("Success:", data);
        return data;
      })
      .catch((error) => {
        console.error("Error storing image:", error);
        throw error;
      });
  };

  // File change handler for image/video upload
  const handleFileChange = async (e) => {
    const file = e.target.files[0];
    setSelectedFile(file);
    const reader = new FileReader();
    reader.onloadend = () => {
      setFilePreview(reader.result);
      setfileType(file.type);
    };
    reader.readAsDataURL(file);
  };

  // Fetch posts for the current topic
  useEffect(() => {
    setIsLoading(true);
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token found; user not logged in?");
      setIsLoading(false);
      return;
    }
    fetch(
      `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getPosts?topicId=${topicId}`,
      { headers: { Authorization: `Bearer ${token}` } }
    )
      .then((response) => {
        if (!response.ok)
          throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
      })
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        if (!Array.isArray(parsedData.data)) {
          console.error("Expected an array but got:", parsedData.data);
          return;
        }
        const filtered = parsedData.data.filter(
          (post) => post.topic_id === parseInt(topicId, 10)
        );
        filtered.forEach((post) => {
          fetchImageFromS3(post.post_id)
            .then((imageData) => {
              if (imageData) {
                setImageURLs((prevURLs) => ({
                  ...prevURLs,
                  [post.post_id]: `data:image/jpeg;base64,${imageData}`
                }));
              }
            })
            .catch((error) => {
              console.error("Error fetching image for post", post.post_id, error);
            });
        });
        setPosts(filtered);
      })
      .catch((error) => console.error("Error fetching posts:", error))
      .finally(() => setIsLoading(false));
  }, [topicId]);

  // Fetch all topics for the left panel
  const [allTopics, setAllTopics] = useState([]);
  useEffect(() => {
    const token = sessionStorage.getItem("idToken");
    if (!token) return;
    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getTopics", {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(response => response.json())
      .then(data => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        if (!Array.isArray(parsedData.data)) return;
        setAllTopics(parsedData.data);
      })
      .catch(error => console.error("Error fetching topics:", error));
  }, []);

  // Fetch all comments for the right panel
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

  const handleInputChange = (e) => {
    setNewPost({ ...newPost, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token found; user not logged in");
      setIsSubmitting(false);
      return;
    }
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const userID = tokenPayload.sub;

    const postData = {
      user_id: userID,
      topic_id: topicId,
      name: newPost.name,
      content: newPost.content
    };

    fetch("https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/newPost", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(postData)
    })
      .then((response) => {
        if (!response.ok)
          throw new Error(`HTTP error! status: ${response.status}`);
        return response.json();
      })
      .then((postResponse) => {
        const newPostId =
          postResponse.postId ||
          (postResponse.body && JSON.parse(postResponse.body).postId) ||
          postResponse.post_id ||
          (postResponse.body && JSON.parse(postResponse.body).post_id);
        if (!newPostId)
          throw new Error("No post id returned from newPost API");
        if (selectedFile) {
          return storeImageToS3(newPostId).then(() => newPostId);
        }
        return newPostId;
      })
      .then((newPostId) => {
        return fetch(
          `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getPosts?topicId=${topicId}`,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      })
      .then((response) => response.json())
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        if (!Array.isArray(parsedData.data)) {
          console.error("Expected an array but got:", parsedData.data);
          return;
        }
        const filtered = parsedData.data.filter(
          (post) => post.topic_id === parseInt(topicId, 10)
        );
        setPosts(filtered);
        setShowCreatePost(false);
        setNewPost({ name: "", content: "" });
        setSelectedFile(null);
        setFilePreview(null);

        filtered.forEach((post) => {
          fetchImageFromS3(post.post_id)
            .then((imageData) => {
              if (imageData) {
                setImageURLs((prevURLs) => ({
                  ...prevURLs,
                  [post.post_id]: `data:image/jpeg;base64,${imageData}`
                }));
              }
            })
            .catch((error) => {
              console.error("Error fetching image for post", post.post_id, error);
            });
        });
      })
      .catch((error) => console.error("Error after creating post:", error))
      .finally(() => setIsSubmitting(false));
  };

  const filteredPosts = posts.filter((post) =>
    post.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const sortedPosts = [...filteredPosts].sort((a, b) => {
    const dateA = new Date(a.created_at);
    const dateB = new Date(b.created_at);
    return sortOrder === "newest" ? dateB - dateA : dateA - dateB;
  });

  return (
    <>
      {/* Left floating panel: Topics */}
      <Box
        sx={{
          position: "fixed",
          left: 0,
          top: "64px", // offset for navbar
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
          Topics
        </Typography>
        <List>
          {allTopics.map((topic) => (
            <ListItem
              key={topic.topic_id}
              button
              component={Link}
              to={`/topic/${topic.topic_id}`}
              state={{ topicName: topic.name }}
            >
              <ListItemText primary={topic.name} />
            </ListItem>
          ))}
        </List>
      </Box>

      {/* Right floating panel: All Comments */}
      <Box
        sx={{
          position: "fixed",
          right: 0,
          top: "64px", // offset for navbar
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

      {/* Main content with margins */}
      <Container
        maxWidth="md"
        sx={{
          mt: "80px", // added top margin to clear the floating navbar
          ml: "260px",
          mr: "260px",
          position: "relative"
        }}
      >
        {isLoading ? (
          <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            <Typography variant="h4" gutterBottom>
              Posts for {topicName}
            </Typography>

            <Box sx={{ mb: 2, display: "flex", gap: 2 }}>
              <TextField
                label="Search posts by title"
                variant="outlined"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                fullWidth
              />
              <TextField
                select
                label="Sort by"
                value={sortOrder}
                onChange={(e) => setSortOrder(e.target.value)}
                variant="outlined"
                sx={{ width: "200px" }}
              >
                <MenuItem value="newest">Newest First</MenuItem>
                <MenuItem value="oldest">Oldest First</MenuItem>
              </TextField>
            </Box>

            <List sx={{ mb: 4 }}>
              {sortedPosts.map((post) => {
                const formattedDate = post.created_at
                  ? format(new Date(post.created_at), "dd MMM yyyy, h:mm a")
                  : "Unknown Date";
                return (
                  <Paper key={post.post_id} sx={{ mb: 2, p: 2 }}>
                    <Typography
                      variant="subtitle2"
                      color="text.secondary"
                      component={Link}  // Make it a clickable link
                      to={`/user/${post.username}`}  // Navigate to user page
                      sx={{ textDecoration: "none", color: "inherit", cursor: "pointer" }} // Style link
                    >
                      {post.username} • {formattedDate}
                    </Typography>
                    <Box sx={{ mt: 3 }}>
                      {imageURLs[post.post_id] && (
                        <img
                          src={imageURLs[post.post_id]}
                          alt={`Post ${post.post_id}`}
                          style={{ width: "300px" }}
                        />
                      )}
                    </Box>
                    <ListItem
                      button
                      component={Link}
                      to={`/post/${post.post_id}`}
                      // state={{ postTitle: post.name, postContent: post.content }}
                      state={{
                        postTitle: post.name,
                        postContent: post.content,
                        topicId: post.topic_id ,
                        username: post.username,
                        date: formattedDate// pass the topic id
                      }}
                    >
                      <ListItemText primary={post.name} secondary={post.content} />
                    </ListItem>
                  </Paper>
                );
              })}
            </List>

            {/* Floating FAB for create post (repositioned) */}
            <Box
            
            sx={{
              position: "fixed",
              bottom: 16,
              left: "50%",
              transform: "translateX(-50%)"
            }}
            >
              {showCreatePost ? (
                <Button
                  variant="contained"
                  color="secondary"
                  onClick={() => setShowCreatePost(false)}
                  startIcon={<CloseIcon />}
                >
                  Close
                </Button>
              ) : (
                <Fab color="primary" onClick={() => setShowCreatePost(true)}>
                  <AddIcon />
                </Fab>
              )}
            </Box>

            {showCreatePost && (
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
                  Create Post
                </Typography>
                <Box
                  component="form"
                  onSubmit={handleSubmit}
                  sx={{ display: "flex", flexDirection: "column", gap: 2 }}
                >
                  <TextField
                    label="Post Title"
                    name="name"
                    value={newPost.name}
                    onChange={handleInputChange}
                    required
                  />
                  <TextField
                    label="Content"
                    name="content"
                    value={newPost.content}
                    onChange={handleInputChange}
                    multiline
                    minRows={3}
                    required
                  />
                  <Button
                    variant="contained"
                    component="label"
                    sx={{ width: "fit-content" }}
                  >
                    Upload Image/Video
                    <input
                      type="file"
                      accept="image/*, video/*"
                      hidden
                      onChange={handleFileChange}
                    />
                  </Button>
                  {filePreview && selectedFile && (
                    <Box
                      component="img"
                      src={filePreview}
                      alt="Preview"
                      width="300px"
                    />
                  )}
                  <Button variant="contained" type="submit" disabled={isSubmitting}>
                    {isSubmitting ? <CircularProgress size={24} /> : "Submit"}
                  </Button>
                </Box>
              </Box>
            )}
          </>
        )}
      </Container>
    </>
  );
};

export default TopicPage;
