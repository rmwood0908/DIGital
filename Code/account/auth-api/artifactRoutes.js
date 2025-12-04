// imports
import express from 'express';
import { pool } from './db.js';

const router = express.Router();

// post route to create a new artifact
router.post('/', async (req, res) => {
    try {
        const {
            date_discovered,
            investigator,
            area,
            unit,
            layer,
            site,
            associated_features,
            material_type,
            quantity,
            weight, 
            bag_number,
            artifact_id
        } = req.body;

        // validate user input (required fields)
        if (
            !date_discovered ||
            !investigator ||
            !area ||
            !unit ||
            !layer ||
            !site ||
            !material_type ||
            quantity === undefined ||
            quantity === '' ||
            weight === undefined ||
            weight === '' ||
            !bag_number ||
            !artifact_id
        ) {
        return res.status(400).json({
            ok: false,
            error: 'All fields are required',
        });
    }

    // coerce quantity and weight to numbers
    const quantityValue = Number(quantity);
    const weightValue = Number(weight);

    if (Number.isNaN(quantityValue) || Number.isNaN(weightValue)) {
      return res.status(400).json({
        ok: false,
        error: 'Quantity and weight must be numbers',
      });
    }

    // insert new artifact into database
    const insertArtifactQuery = 
        `INSERT INTO artifacts 
        (date_discovered, investigator, area, unit, layer, site, 
         associated_features, material_type, quantity, weight, 
         bag_number, artifact_id) 
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12)
         RETURNING id`;

    const values = [
        date_discovered,
        investigator,
        area,
        unit,
        layer,
        site,
        associated_features || null,
        material_type,
        quantityValue,
        weightValue,
        bag_number,
        artifact_id
    ];

    // send query to database
    const result = await pool.query(insertArtifactQuery, values);

    // respond with success or error message
    res.status(201).json({ ok: true,
                            message: 'Artifact created successfully',
                            artifactId: result.rows[0].id });
    } catch (error) {
        console.error('Error creating artifact:', error);
        res.status(500).json({ ok: false, 
                               error: 'Internal server error' });
    }
});

// get route to show artifact to analyze
router.get('/latest', async (req, res) => {
    try {
        const selectArtifactQuery = 
        `SELECT id, date_discovered, investigator, area,
                unit, layer, site, associated_features,
                material_type, quantity, weight, bag_number,
                artifact_id
         FROM artifacts
         ORDER BY created_at DESC
         LIMIT 1;`;

        // send query to database
        const result = await pool.query(selectArtifactQuery);

        // error handling
        if( result.rows.length === 0 ) {
            return res.status(404).json({
                ok: false, error: 'No artifact found',
            });
        }

        return res.status(200).json({
            ok: true, artifact: result.rows[0],
        });
    }
    catch( error ){
        console.error('Error fetching latest artifact:', error);
        return res.status(500).json({
            ok: false,
            error: 'Internal server error.',
        });
    }
});

// get all artifacts (for dropdown in analysis scene)
router.get('/', async (req, res) => {
    try {
        const selectAllArtifactsQuery =
        `SELECT id, date_discovered, investigator,
                area, unit, layer, site, associated_features,
                material_type, quantity, weight, bag_number,
                artifact_id
         FROM artifacts
         ORDER BY created_at DESC;`;

        // send query to database
        const result = await pool.query(selectAllArtifactsQuery);

        // error handling
        return res.status(200).json({
            ok: true, artifacts: result.rows, error: null,
        });
    }
    catch(error) {
        console.error('Error fetching artifacts list:', error);
        return res.status(500).json({
            ok: false, artifacts: [], error: "Internal server error.",
        });
    }
});

// export the router
export default router;