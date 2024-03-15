"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.MintController = void 0;
const Openfort = require('@openfort/openfort-node').default;
class MintController {
    constructor() {
        this.run = this.run.bind(this);
    }
    run(req, res, next) {
        return __awaiter(this, void 0, void 0, function* () {
            const openfort = new Openfort(process.env.OPENFORT_SECRET_KEY);
            const auth = req.headers.authorization;
            if (!auth) {
                return res.status(401).json({ error: "No authorization header" });
            }
            const token = auth.split("Bearer ")[1];
            if (!token) {
                return res.status(401).json({ error: "No token" });
            }
            const player = yield openfort.iam.verifyAuthToken(token);
            if (!player) {
                return res.status(401).json({ error: "Invalid token" });
            }
            const policy_id = "pol_0b74cbac-146b-4a1e-98e1-66e83aef5deb";
            const contract_id = "con_42883506-04d5-408e-93da-2151e293a82b";
            const chainId = 80001;
            const interaction_mint = {
                contract: contract_id,
                functionName: "mint",
                functionArgs: [player.playerId],
            };
            try {
                console.log("transactionIntent");
                const transactionIntent = yield openfort.transactionIntents.create({
                    player: player.playerId,
                    policy: policy_id,
                    chainId,
                    interactions: [interaction_mint],
                });
                console.log("transactionIntent", transactionIntent);
                return res.send({
                    data: transactionIntent,
                });
            }
            catch (e) {
                console.log(e);
                return res.send({
                    data: null,
                });
            }
        });
    }
}
exports.MintController = MintController;
