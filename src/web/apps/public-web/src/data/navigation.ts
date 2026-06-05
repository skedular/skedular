import { ctas } from "./ctas";
import { primaryRoutes, utilityRoutes } from "./routes";

export const primaryNavigation = primaryRoutes;

export const footerNavigation = [
  ...primaryRoutes,
  ...utilityRoutes,
  { id: "book-demo", label: "Book Demo", path: ctas.find((cta) => cta.id === "book-demo")?.destinationRef ?? "PUBLIC_SKEDULAR_DEMO_URL" },
  { id: "login", label: "Login", path: ctas.find((cta) => cta.id === "login")?.destinationRef ?? "PUBLIC_SKEDULAR_SIGNUP_URL" },
] as const;
