import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import PostPage from "./pages/PostPage";

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
  // Set a fake token with a proper JWT structure.
  const fakeToken =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
    btoa(JSON.stringify({ sub: "test-sub" })) +
    ".signature";
  sessionStorage.setItem("idToken", fakeToken);
});

describe("PostPage Component", () => {
  test("renders post title", async () => {
    render(
      <MemoryRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <PostPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText(/Unknown Title/i)).toBeInTheDocument();
    });
  });

  test("renders Add a Comment input", async () => {
    render(
      <MemoryRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <PostPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByLabelText(/Your comment/i)).toBeInTheDocument();
    });
  });

  test("allows typing in comment input", async () => {
    render(
      <MemoryRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <PostPage />
      </MemoryRouter>
    );
    const input = await screen.findByLabelText(/Your comment/i);
    fireEvent.change(input, { target: { value: "This is a test comment." } });
    expect(input.value).toBe("This is a test comment.");
  });
});
