from fastapi import FastAPI
from pydantic import BaseModel
import joblib
import pandas as pd

app = FastAPI()

model = joblib.load("sales_forecast_model.pkl")

class ForecastRequest(BaseModel):
    product_id: int
    month: int
    year: int

class YearlyForecastRequest(BaseModel):
    product_id: int
    year: int

@app.post("/predict")
def predict_quantity(req: ForecastRequest):
    X = pd.DataFrame([{
        "ProductId": req.product_id,
        "Year": req.year,
        "Month": req.month
    }])
    prediction = model.predict(X)[0]
    return {"predicted_quantity": round(prediction)}

@app.post("/predict/yearly")
def predict_yearly(req: YearlyForecastRequest):
    forecast = []
    for month in range(1, 13):
        X = pd.DataFrame([{
            "ProductId": req.product_id,
            "Year": req.year,
            "Month": month
        }])
        prediction = model.predict(X)[0]
        forecast.append({
            "month": month,
            "year": req.year,
            "predicted_quantity": round(prediction)
        })
    return {
        "product_id": req.product_id,
        "forecast": forecast
    }
