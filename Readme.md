# Google Ads Reporting Tool

A **Windows Forms application** built with **C# and .NET** that automates extraction of keyword-level click and engagement data from Google Ads campaigns. Ideal for marketing analytics and performance reporting, this project demonstrates integration with the Google Ads API and automated report generation.

---

## 🚀 Project Overview

This tool allows you to:

- Retrieve all **active campaigns** for a specified Google Ads customer account.
- Generate detailed keyword reports including:
  - Clicks
  - Impressions
  - Interactions
  - Campaign, Ad Group, and Date information
- Export reports as **tab-delimited `.txt` files**.
- Create reports for:
  - A **specific date**
  - A **specific month**
  - A **custom month range**
- Refresh **OAuth2 tokens** directly from the application.

> 💡 This is a **sanitized version** for public sharing. All sensitive credentials are loaded from `App.config` and are **not included** in the repository.

---

## 🎯 Key Features

- **Automated Campaign Retrieval:** Fetch all active campaigns programmatically.
- **Keyword-Level Reporting:** Capture metrics at the most granular level.
- **Flexible Timeframes:** Generate reports by day, month, or custom ranges.
- **File Output:** Saves reports in a configurable directory.
- **OAuth2 Integration:** Securely handles Google Ads API authorization.

---

## 📷 Screenshots

*[App-UI](app-ui.PNG)*  

---

## 🛠 Tech Stack

- **Language:** C# (.NET Framework)
- **Libraries / APIs:**  
  - Google Ads API (v17)  
  - Google.Apis.Auth  
  - Windows Forms  
- **Tools:** Visual Studio, NuGet

---

## 📂 Sample Output

Reports are saved as tab-delimited text files:

- `ClickData_ReportByDate_dd-MMM-yyyy_createdOn_dd_MMM_yyyy.txt`
- `ClickData_ReportByMonth_MMM-yyyy_createdOn_dd_MMM_yyyy.txt`
- `ClickData_ReportByMonth_MMM-yyyytoMMM-yyyy_createdOn_dd_MMM_yyyy.txt`

**Example content:**

| Date       | CampaignName | AdGroup  | Keyword      | Clicks | Interactions | Impressions |
|------------|--------------|---------|-------------|--------|--------------|-------------|
| 2025-12-01 | Winter Promo | AdGrp1  | winter123   | 12     | 15           | 1200        |
| 2025-12-01 | Winter Promo | AdGrp2  | snowgear456 | 8      | 10           | 900         |

---

## 💡 Why Employers Will Like This

- Demonstrates **API integration skills** with Google Ads.
- Shows **automation & reporting expertise** in a production-like project.
- Exhibits **clean, modular code** and Windows Forms application design.
- Highlights **data handling** and **file I/O operations** for analytics.

---

## 🔒 Read‑Only Usage Notice

This repository is provided solely for evaluation by potential employers. You may view and review the code to assess engineering ability.

**All other actions are not permitted**, including:

- Reuse or modification
- Redistribution or publication
- Integration into any software
- Commercial or operational use
- Automation of any real-world systems

A full license file in this repository reinforces these restrictions.

---

## 📌 Notes

This code is intentionally incomplete and non‑functional in this public version. Its purpose is to demonstrate architecture, coding style, and automation expertise, **not to operate against any real software**.


