"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const express = require("express");
const api_1 = require("./api");
const app = express();
const port = 4000;
app.post('/mint', new api_1.MintController().run);
app.listen(port, () => {
    console.log(`Server is running at http://localhost:${port}`);
});
