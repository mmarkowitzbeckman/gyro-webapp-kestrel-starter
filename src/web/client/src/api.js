// Minimal GraphQL client. Calls the same-origin /graphql endpoint (under the app's
// base path), which is proxied to the API server by Vite in dev and by the Kestrel
// web server in production.
export const graphqlRequest = async (query, variables = {}) => {
  const response = await fetch(`${import.meta.env.BASE_URL}graphql`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ query, variables }),
  });

  const { data, errors } = await response.json();
  if (errors?.length) {
    throw new Error(errors.map((e) => e.message).join("; "));
  }
  return data;
};

export const healthQuery = `
  query health {
    health
  }
`;
