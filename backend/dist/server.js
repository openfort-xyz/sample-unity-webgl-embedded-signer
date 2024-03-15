"use strict";
var _a;
Object.defineProperty(exports, "__esModule", { value: true });
const express = require("express");
const api_1 = require("./api");
const dotenv = require("dotenv");
const app = express();
const PORT = (_a = process.env.PORT) !== null && _a !== void 0 ? _a : 3000;
dotenv.config();
if (!process.env.OPENFORT_SECRET_KEY) {
    throw new Error(`Unable to load the .env file. Please copy .env.example to .env and fill in the required environment variables.`);
}
app.get("/", (req, res) => {
    res.send("Service is running");
});
app.post('/mint', new api_1.MintController().run);
app.listen(PORT, () => {
    console.log(`Server is running at http://localhost:${PORT}`);
});
