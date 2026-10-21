import { render, screen, waitFor } from "@testing-library/react";
import { App } from "../App";

beforeEach(() => {
  global.fetch = jest.fn();
});

test("renders the health check result once the API responds", async () => {
  global.fetch.mockResolvedValue({ json: () => Promise.resolve({ data: { health: "ok" } }) });

  render(<App />);

  expect(screen.getByText("Checking…")).toBeInTheDocument();
  await waitFor(() => expect(screen.getByText("ok")).toBeInTheDocument());
});

test("shows an error if the health check fails", async () => {
  global.fetch.mockResolvedValue({ json: () => Promise.resolve({ errors: [{ message: "network down" }] }) });

  render(<App />);

  await waitFor(() => expect(screen.getByText("Error: network down")).toBeInTheDocument());
});
