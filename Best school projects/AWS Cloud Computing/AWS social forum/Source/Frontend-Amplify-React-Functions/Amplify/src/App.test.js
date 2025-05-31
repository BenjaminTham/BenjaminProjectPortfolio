import { render, screen } from "@testing-library/react";
import App from "./App";

beforeEach(() => {
  const fakeToken =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
    btoa(JSON.stringify({ sub: "test-sub" })) +
    ".signature";
  sessionStorage.setItem("idToken", fakeToken);
});

test("renders forum heading", () => {
  render(<App />);
  // Update the matcher to the actual text in the header.
  const heading = screen.getByText(/Joo's Amazing Forum/i);
  expect(heading).toBeInTheDocument();
});
