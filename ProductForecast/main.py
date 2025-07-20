from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from model.model_loader import load_model
import numpy as np

app = FastAPI(title="Sales Forecast API")

model = load_model()

class PredictionInput(BaseModel):
    product_id: int
    year: int
    month: int

@app.post("/predict")
def predict_quantity(input_data: PredictionInput):
    try:
        X = np.array([[input_data.product_id, input_data.year, input_data.month]])
        predicted_quantity = model.predict(X)[0]
        return {"predicted_quantity": round(predicted_quantity)}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
