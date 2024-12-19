module.exports = {
  src: "./src",
  schema: "../../../api-definitions/graphql/skedular/v1/schema.graphql",
  exclude: ["**/node_modules/**", "**/__mocks__/**", "**/__generated__/**"],
  language: "typescript",
  artifactDirectory: "src/queries/__generated__",
};
