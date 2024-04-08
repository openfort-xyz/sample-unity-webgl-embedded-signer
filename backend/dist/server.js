"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var _a;
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const cors_1 = __importDefault(require("cors")); // Import the cors middleware
const api_1 = require("./api");
const dotenv_1 = __importDefault(require("dotenv"));
const app = (0, express_1.default)();
const PORT = (_a = process.env.PORT) !== null && _a !== void 0 ? _a : 3000;
dotenv_1.default.config();
if (!process.env.OPENFORT_SECRET_KEY) {
    throw new Error(`Unable to load the .env file. Please copy .env.example to .env and fill in the required environment variables.`);
}
// Use the cors middleware to disable CORS
app.use((0, cors_1.default)());
app.get("/", (req, res) => {
    res.send("Service is running");
});
app.post('/mint', new api_1.MintController().run);
app.listen(PORT, () => {
    console.log(`Server is running at http://localhost:${PORT}`);
});
