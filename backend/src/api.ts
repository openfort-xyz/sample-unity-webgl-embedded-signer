import { type NextFunction, type Request, type Response } from "express";
const Openfort = require('@openfort/openfort-node').default;
export class MintController {

    constructor() {
        this.run = this.run.bind(this);
    }

    async run(req: Request, res: Response, next: NextFunction) {

        const openfort = new Openfort(process.env.OPENFORT_SECRET_KEY);
        const auth = req.headers.authorization;
        if (!auth) {
            return res.status(401).json({ error: "No authorization header" });
        }

        const token = auth.split("Bearer ")[1];
        if (!token) {
            return res.status(401).json({ error: "No token" });
        }


        const player = await openfort.iam.verifyAuthToken(token)
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
            console.log("transactionIntent")
            const transactionIntent = await openfort.transactionIntents.create({
                player: player.playerId,
                policy: policy_id,
                chainId,
                interactions: [interaction_mint],
            });

            console.log("transactionIntent", transactionIntent)
            return res.send({
                data: transactionIntent,
            });
        } catch (e: any) {
            console.log(e);
            return res.send({
                data: null,
            });
        }

    }
}
