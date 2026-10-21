import { useEffect, useState } from "react";
import { graphqlRequest, healthQuery } from "./api";
import styles from "./App.module.css";

export const App = () => {
  const [status, setStatus] = useState("Checking…");

  useEffect(() => {
    graphqlRequest(healthQuery)
      .then(({ health }) => setStatus(health))
      .catch((err) => setStatus(`Error: ${err.message}`));
  }, []);

  return (
    <main className={styles.app}>
      <h1>Gyroscope Webapp Starter</h1>
      <p>
        API health check: <span className={styles.statusText}>{status}</span>
      </p>
    </main>
  );
};
