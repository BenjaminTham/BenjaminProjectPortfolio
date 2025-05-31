import React, { useState, useEffect } from "react";
import { format } from "date-fns"
import { useParams, useLocation , Link } from "react-router-dom";
import {
  Container,
  Typography,
  Button,
  TextField,
  Paper,
  List,
  ListItem,
  ListItemText,
  Box,
  CircularProgress
} from "@mui/material";

const PostPage = () => {
  const { postId } = useParams();
  const location = useLocation();
  const postTitle = location.state?.postTitle || "Unknown Title";
  const postContent = location.state?.postContent || "No content available";
  const username = location.state?.username || "Unknown";
  const date = location.state?.date || "Unknown date";

  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const [selectedFile, setSelectedFile] = useState(null);
  const { topicId } = location.state || {};

  const [imageURL, setImageURL] = useState(null);
  const [filePreview, setFilePreview] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [loggedInUserId, setLoggedInUserId] = useState(null);

  // On mount, fetch comments for this post
  // useEffect(() => {
  //   setIsLoading(true);

  //   const token = sessionStorage.getItem("idToken");
  //   // If no token, you can decide to skip fetch or show a "please login" message
  //   if (!token) {
  //     console.error("No token in sessionStorage; user is not logged in?");
  //     setIsLoading(false);
  //     return;
  //   }

  //   const tokenPayload = JSON.parse(atob(token.split(".")[1]));
  //   const userID = tokenPayload.sub;
  //   setLoggedInUserId(userID);

  //   fetch(
  //     `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments?post_id=${postId}`,
  //     { headers: { Authorization: `Bearer ${token}` } }
  //   )
  //     .then((response) => response.json())
  //     .then((data) => {
  //       const parsedData = data.body ? JSON.parse(data.body) : data;
  //       let allComments = [];
  //       if (Array.isArray(parsedData.comments)) {
  //         allComments = parsedData.comments;
  //       } else if (Array.isArray(parsedData.data)) {
  //         allComments = parsedData.data;
  //       }
  //       // Filter comments that match this post
  //       const filtered = allComments.filter(
  //         (comment) => comment.post_id === parseInt(postId, 10)
  //       );
  //       setComments(filtered);
  //     })
  //     .catch((error) => console.error("Error fetching comments:", error))
  //     .finally(() => setIsLoading(false));
  // }, [postId]);

  useEffect(() => {
    setIsLoading(true);
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token in sessionStorage; user is not logged in?");
      setIsLoading(false);
      return;
    }
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const userID = tokenPayload.sub;
    setLoggedInUserId(userID);

    fetch(
      `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments?post_id=${postId}`,
      { headers: { Authorization: `Bearer ${token}` } }
    )
      .then((response) => response.json())
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        let allComments = [];
        if (Array.isArray(parsedData.comments)) {
          allComments = parsedData.comments;
        } else if (Array.isArray(parsedData.data)) {
          allComments = parsedData.data;
        }
        const filtered = allComments.filter(
          (comment) => comment.post_id === parseInt(postId, 10)
        );
        setComments(filtered);
      })
      .catch((error) => console.error("Error fetching comments:", error))
      .finally(() => setIsLoading(false));
  }, [postId]);


  useEffect(() => {
    if (postId && topicId) {
      fetchImageFromS3(postId, topicId)
        .then((fileContent) => {
          if (fileContent) {
            setImageURL(`data:image/jpeg;base64,${fileContent}`);
          }
        })
        .catch((error) => {
          console.error("Error retrieving image for post:", error);
        });
    }
  }, [postId, topicId]);

  // Editing a comment
  const handleCommentEdit = (commentId, currentContent) => {
    const updatedContent = prompt("Edit your comment:", currentContent);
    if (!updatedContent) return;

    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token found; user not logged in");
      return;
    }

    // You may decode to find user_id if your Lambda requires it
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const userID = tokenPayload.sub;

    const payload = {
      user_id: userID,
      comment_id: commentId,
      content: updatedContent
    };
    console.log(payload);

    fetch(
      "https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/updateComment",
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(payload)
      }
    )
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then(() => {
        // re-fetch comments after posting
        return fetch(
          `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments?post_id=${postId}`,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      })
      .then((res) => res.json())
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        let allComments = [];
        if (Array.isArray(parsedData.comments)) {
          allComments = parsedData.comments;
        } else if (Array.isArray(parsedData.data)) {
          allComments = parsedData.data;
        }
        const filtered = allComments.filter(
          (comment) => comment.post_id === parseInt(postId, 10)
        );
        setComments(filtered);
        setNewComment("");
      })
      .catch((error) => console.error("Error updating comment:", error));
    
    
    
  };

  // Deleting a comment
  const handleCommentDelete = (commentId) => {
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token found; user not logged in");
      return;
    }

    // Optionally decode the token to get user_id if your Lambda needs it
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const userID = tokenPayload.sub;

    const payload = {
      user_id: userID,
      comment_id: commentId
    };

    fetch(
      "https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/deleteComment",
      {
        method: "DELETE",
        headers: { 
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
         },
        body: JSON.stringify(payload)
      }
    )
    .then((response) => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then(() => {
      // re-fetch comments after deleting
      return fetch(
        `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments?post_id=${postId}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
    })
    .then((res) => res.json())
    .then((data) => {
      const parsedData = data.body ? JSON.parse(data.body) : data;
      let allComments = [];
      if (Array.isArray(parsedData.comments)) {
        allComments = parsedData.comments;
      } else if (Array.isArray(parsedData.data)) {
        allComments = parsedData.data;
      }
      const filtered = allComments.filter(
        (comment) => comment.post_id === parseInt(postId, 10)
      );
      setComments(filtered);
      setNewComment("");
    })
      .catch((error) => console.error("Error deleting comment:", error));
  };

  // Handling file upload
  const handleFileChange = async (e) => {
    const file = e.target.files[0];
    setSelectedFile(file);

    const reader = new FileReader();
    reader.onloadend = () => {
      const fileContent = reader.result.split(",")[1];
      setFilePreview(reader.result);

      const formData = new FormData();
      formData.append("content", fileContent);
      formData.append("file_name", file.name);

      fetch(
        "https://pwsgthrir2.execute-api.us-east-1.amazonaws.com/test-stage/upload-data-s3",
        { method: "POST", body: formData }
      )
        .then((response) => response.json())
        .then((data) => console.log("Success:", data))
        .catch((error) => console.error("Error:", error));
    };
    reader.readAsDataURL(file);
  };

  const fetchImageFromS3 = async (postID, topicID) => {
    // Construct the S3 file name as "topicId_postId"
    const s3Name = `${topicID}_${postID}`;
    try {
      const response = await fetch(
        "https://h2ngxg46k3.execute-api.ap-southeast-2.amazonaws.com/test-stage/retrieve-s3",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ partial_name: s3Name })
        }
      );
      if (response.status === 404) return null;
      if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
      const data = await response.json();
      const parsedData = data.body ? JSON.parse(data.body) : data;
      return parsedData.file_content || null;
    } catch (error) {
      console.error("Error fetching image:", error);
      return null;
    }
  };

  // Submitting a new comment
  const handleNewCommentSubmit = (e) => {
    e.preventDefault();
    const token = sessionStorage.getItem("idToken");
    if (!token) {
      console.error("No token found; user is not logged in");
      return;
    }

    // Decode to get userID
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const userID = tokenPayload.sub;

    const payload = {
      user_id: userID,
      post_id: postId,
      content: newComment
    };

    fetch(
      "https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/newComment",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(payload)
      }
    )
      .then((response) => response.json())
      .then(() => {
        // re-fetch comments after posting
        return fetch(
          `https://6kz844frt5.execute-api.us-east-1.amazonaws.com/dev/getComments?post_id=${postId}`,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      })
      .then((res) => res.json())
      .then((data) => {
        const parsedData = data.body ? JSON.parse(data.body) : data;
        let allComments = [];
        if (Array.isArray(parsedData.comments)) {
          allComments = parsedData.comments;
        } else if (Array.isArray(parsedData.data)) {
          allComments = parsedData.data;
        }
        const filtered = allComments.filter(
          (comment) => comment.post_id === parseInt(postId, 10)
        );
        setComments(filtered);
        setNewComment("");
      })
      .catch((error) => console.error("Error adding comment:", error));
  };

//   return (
//     <Container maxWidth="md" sx={{ mt: 4 }}>
//       {isLoading ? (
//         <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
//           <CircularProgress />
//         </Box>
//       ) : (
//         <>
//           <Typography variant="h4" gutterBottom>
//             {postTitle}
//           </Typography>
//           <Typography variant="body1" sx={{ mb: 3 }}>
//             {postContent}
//           </Typography>

//           <Typography variant="h5" gutterBottom>
//             Comments
//           </Typography>
//           <List sx={{ mb: 3 }}>
//             {comments.map((comment) => {
//               // Format the timestamp to a readable format (12-hour with AM/PM)
//               const formattedDate = comment.created_at
//                 ? format(new Date(comment.created_at), "dd MMM yyyy, h:mm a") // 12-hour format
//                 : "Unknown Date";

//               return (
//                 <Paper key={comment.id} sx={{ mb: 2, p: 2 }}>
//                   {/* Display Username and Formatted Timestamp */}
//                   <Typography variant="subtitle2" color="text.secondary">
//                     {comment.username} • {formattedDate}
//                   </Typography>

//                   {/* Comment Content */}
//                   <ListItem disablePadding>
//                     <ListItemText primary={comment.content} />
//                   </ListItem>

//                   {/* Show Edit and Delete buttons only if the comment belongs to the logged-in user */}
//                   {comment.user_id === loggedInUserId && (
//                     <Box sx={{ mt: 1 }}>
//                       {/* Edit Button */}
//                       <Button
//                         variant="outlined"
//                         size="small"
//                         onClick={() => handleCommentEdit(comment.comment_id, comment.content)}
//                         sx={{ mr: 1 }}
//                       >
//                         Edit
//                       </Button>
//                       {/* Delete Button */}
//                       <Button
//                         variant="outlined"
//                         color="error"
//                         size="small"
//                         onClick={() => handleCommentDelete(comment.comment_id)}
//                       >
//                         Delete
//                       </Button>
//                     </Box>
//                   )}
//                 </Paper>

//               );
//             })}
//           </List>

//           <Typography variant="h6" gutterBottom>
//             Add a Comment
//           </Typography>
//           <Box
//             component="form"
//             onSubmit={handleNewCommentSubmit}
//             sx={{ display: "flex", flexDirection: "column", gap: 2 }}
//           >
//             {filePreview && (
//               <>
//                 {selectedFile?.type.startsWith("image/") ? (
//                   <Box
//                     component="img"
//                     src={filePreview}
//                     alt="Selected Preview"
//                     width="300px"
//                   />
//                 ) : selectedFile?.type.startsWith("video/") ? (
//                   <video width="300" controls>
//                     <source src={filePreview} type={selectedFile.type} />
//                     Your browser does not support the video tag.
//                   </video>
//                 ) : (
//                   <Typography color="text.secondary">Unsupported file type</Typography>
//                 )}
//               </>
//             )}

//             <Button variant="contained" component="label" sx={{ width: "fit-content" }}>
//               Upload Image/Video
//               <input
//                 type="file"
//                 accept="image/*, video/*"
//                 hidden
//                 onChange={handleFileChange}
//               />
//             </Button>

//             <TextField
//               label="Your comment"
//               multiline
//               minRows={3}
//               value={newComment}
//               onChange={(e) => setNewComment(e.target.value)}
//               required
//             />

//             <Button variant="contained" type="submit">
//               Post Comment
//             </Button>
//           </Box>

//           {/* <Box sx={{ mt: 3 }}>
//             {imageURL ? (
//               <img src={imageURL} alt="Fetched from S3" style={{ width: "300px" }} />
//             ) : (
//               // <Button variant="outlined" onClick={fetchImageFromS3}>
//               //   Fetch Image from S3
//               // </Button>
//             )}
//           </Box> */}
//         </>
//       )}
//     </Container>
//   );
// };

// export default PostPage;



return (
  <Container maxWidth="md" sx={{ mt: "80px" }}>
    {isLoading ? (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
        <CircularProgress />
      </Box>
    ) : (
      <>
      {/* Display username and formatted date */}
      <Typography variant="subtitle1" color="text.secondary" sx={{ mb: 1 }}>
          Posted by {username} • {date}
        </Typography>
        
        <Typography variant="h4" gutterBottom>
          {postTitle}
        </Typography>
        <Typography variant="body1" sx={{ mb: 3 }}>
          {postContent}
        </Typography>
        {imageURL && (
            <Box sx={{ mb: 3 }}>
              {/* <img src={imageURL} alt="Post visual" style={{ width: "300px" }} /> */}
              <img
  src={imageURL}
  alt="Post visual"
  style={{ width: "1000px", maxWidth: "100%" }}
/>


            </Box>
          )}

        <Typography variant="h5" gutterBottom>
          Comments
        </Typography>
        <List sx={{ mb: 3 }}>
          {comments.map((comment) => {
            const formattedDate = comment.created_at
              ? format(new Date(comment.created_at), "dd MMM yyyy, h:mm a")
              : "Unknown Date";
            return (
              <Paper key={comment.id} sx={{ mb: 2, p: 2 }}>
          
                <Typography
                  variant="subtitle2"
                  color="text.secondary"
                  component={Link}  // Make it a clickable link
                  to={`/user/${comment.username}`}  // Navigate to user page
                  sx={{ textDecoration: "none", color: "inherit", cursor: "pointer" }} // Style link
                >
                  {comment.username} • {formattedDate}
                </Typography>
                <ListItem disablePadding>
                  <ListItemText primary={comment.content} />
                </ListItem>
                {comment.user_id === loggedInUserId && (
                  <Box sx={{ mt: 1 }}>
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => handleCommentEdit(comment.comment_id, comment.content)}
                      sx={{ mr: 1 }}
                    >
                      Edit
                    </Button>
                    <Button
                      variant="outlined"
                      color="error"
                      size="small"
                      onClick={() => handleCommentDelete(comment.comment_id)}
                    >
                      Delete
                    </Button>
                  </Box>
                )}
              </Paper>
            );
          })}
        </List>

        <Typography variant="h6" gutterBottom>
          Add a Comment
        </Typography>
        <Box
          component="form"
          onSubmit={handleNewCommentSubmit}
          sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
          {filePreview && (
            <>
              {selectedFile?.type.startsWith("image/") ? (
                <Box
                  component="img"
                  src={filePreview}
                  alt="Selected Preview"
                  width="300px"
                />
              ) : selectedFile?.type.startsWith("video/") ? (
                <video width="300" controls>
                  <source src={filePreview} type={selectedFile.type} />
                  Your browser does not support the video tag.
                </video>
              ) : (
                <Typography color="text.secondary">
                  Unsupported file type
                </Typography>
              )}
            </>
          )}
          {/* <Button variant="contained" component="label" sx={{ width: "fit-content" }}>
            Upload Image/Video
            <input
              type="file"
              accept="image/*, video/*"
              hidden
              onChange={handleFileChange}
            />
          </Button> */}
          <TextField
            label="Your comment"
            multiline
            minRows={3}
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            required
          />
          <Button variant="contained" type="submit">
            Post Comment
          </Button>
        </Box>
      </>
    )}
  </Container>
);
};

export default PostPage;