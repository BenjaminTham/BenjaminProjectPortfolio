import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import TopicPage from "./pages/TopicPage";

beforeAll(() => {
  const mockSessionStorage = (() => {
    let store = {};
    return {
      getItem: (key) => store[key] || null,
      setItem: (key, value) => (store[key] = value.toString()),
      removeItem: (key) => delete store[key],
      clear: () => (store = {}),
    };
  })();
  Object.defineProperty(window, "sessionStorage", {
    value: mockSessionStorage,
  });
});

beforeEach(() => {
  // Set a valid fake JWT (with three dot-separated parts)
  const fakeToken =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
    btoa(JSON.stringify({ sub: "test-sub" })) +
    ".signature";
  sessionStorage.setItem("idToken", fakeToken);

  // Mock fetch to simulate a successful API response for getPosts
  global.fetch = jest.fn((url, options) => {
    if (url.includes("/dev/getPosts")) {
      const topicId = new URL(url).searchParams.get("topicId");
      return Promise.resolve({
        ok: true,
        json: () =>
          Promise.resolve({
            body: JSON.stringify({
              data: [
                {
                  post_id: 1,
                  topic_id: parseInt(topicId, 10),
                  name: "Test Post",
                  content: "Test Content",
                  created_at: "2020-01-01T00:00:00Z",
                  username: "TestUser",
                },
              ],
            }),
          }),
      });
    }
    // Default mock for any other fetch calls
    return Promise.resolve({
      ok: true,
      json: () => Promise.resolve({}),
    });
  });
});

describe("TopicPage Component", () => {
  test("renders topic title", async () => {
    render(
      <MemoryRouter
        initialEntries={["/topic/1"]}  // Provide a valid route so that useParams returns topicId "1"
        future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
      >
        <TopicPage />
      </MemoryRouter>
    );
    // Wait for the component to finish loading and the topic title to be rendered.
    await waitFor(() => {
      expect(screen.getByText(/Unknown Topic/i)).toBeInTheDocument();
    });
  });

  test("renders Create a New Post form", async () => {
    render(
      <MemoryRouter
        initialEntries={["/topic/1"]}
        future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
      >
        <TopicPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText(/Create a New Post/i)).toBeInTheDocument();
    });
  });

  test("allows typing in post title input", async () => {
    render(
      <MemoryRouter
        initialEntries={["/topic/1"]}
        future={{ v7_startTransition: true, v7_relativeSplatPath: true }}
      >
        <TopicPage />
      </MemoryRouter>
    );
    // Wait for the form to be rendered before finding the input.
    const input = await screen.findByLabelText(/Post Title/i);
    fireEvent.change(input, { target: { value: "New Test Post" } });
    expect(input.value).toBe("New Test Post");
  });
});
