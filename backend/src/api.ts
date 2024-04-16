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

        const policy_id = "pol_30873bf9-929a-4273-ad4f-48842eea403b";
        const contract_id = "con_8d6b19e8-3a5a-4643-8dee-778997a7dffc";
        const chainId = 80002;

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
