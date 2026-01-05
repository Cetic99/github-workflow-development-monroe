# NFC Card Configurator - Quick Start Guide

## ?? 5-Minute Setup

### Prerequisites Check
1. ? Windows 10 or Windows 11
2. ? NFC Reader (ACR1252U recommended)
3. ? .NET 8.0 Runtime installed
4. ? NFC card to test

---

## Step 1: Verify Hardware

### Connect Your Reader
1. Plug NFC reader into USB port
2. Wait for Windows to install drivers
3. Open **Device Manager** (Win + X ? Device Manager)
4. Look for "Smart card readers" category
5. You should see your reader listed (e.g., "ACS ACR1252 1S CL Reader")

### Check Smart Card Service
1. Press **Win + R**
2. Type `services.msc` and press Enter
3. Find "**Smart Card**" service
4. Status should be "**Running**"
5. If not, right-click ? Start

---

## Step 2: Launch Application

1. Run `NFC Card Configurator.exe`
2. Application window opens with:
   - **Header**: "NFC Card Configurator"
   - **Reader dropdown**: Your reader should appear
   - **Debug log**: Shows initialization messages

### First Launch Checklist
- ? Application opens without errors
- ? Reader appears in dropdown
- ? Debug log shows "? Found 1 reader(s)"
- ? Status: "Reader found and ready"

---

## Step 3: Read Your First Card

### Method 1: Automatic (Recommended)
1. **Select reader** from dropdown (if not already selected)
2. **Place NFC card** on reader
3. Watch as application **automatically**:
   - Detects card
   - Connects to reader
   - Shows UID
   - Enables Read/Write buttons
4. **Click "Read"** button
5. **View results** in popup window

### Method 2: Manual
1. **Place card** on reader first
2. Click "**Connect**" button
3. Wait for green status indicator
4. Click "**Read**" button
5. View comprehensive analysis

---

## Step 4: Understanding the Results

### Quick Reference
```
? Factory State: YES = Card is blank/new
? Factory State: NO = Card has been written

? Lock Status: UNLOCKED = Can write
?? Lock Status: LOCKED = Read-only

? NDEF: YES = Formatted for NFC apps
? NDEF: NO = Raw card data
```

### What You'll See
1. **IDENTIFICATION**
   - UID: Unique card ID
   - Type: Card model/series
   
2. **TECHNICAL SPECS**
   - ATR: Hardware signature
   - Protocol: Communication method
   - Memory: Storage capacity
   
3. **CARD STATUS**
   - Factory state
   - Lock status
   - NDEF support
   - Block statistics
   
4. **DATA CONTENT** (if card has data)
   - NDEF messages
   - Memory dump
   - Hex values

---

## Common First-Time Issues

### ? "No readers available"
**Solution:**
1. Check USB connection
2. Restart application
3. Click "PC/SC Diagnostics"
4. Verify Smart Card service is running

### ? "No card detected"
**Solution:**
1. Remove card and place again
2. Ensure card is centered on reader
3. Try different card to rule out damaged card
4. Check reader LED (should light up with card)

### ? "Error reading card data"
**Solution:**
1. Card may be damaged
2. Card may require authentication (future feature)
3. Check debug log for specific error
4. Try "Connect" button manually

---

## Tips for Best Results

### ?? Card Placement
- Center card over reader antenna
- Keep card flat and still
- Don't remove card during read operation
- Wait for green indicator before reading

### ?? Reading Cards
- Always check debug log for details
- Multiple reads won't damage card
- Read operation takes 1-3 seconds
- Popup shows formatted results

### ?? Saving Results
- Copy text from popup window
- Debug log shows raw read data
- Take screenshot of results
- Future version will have export feature

---

## Next Steps

### Learn More
1. Read full README.md for detailed features
2. Check CHANGELOG.md for version history
3. Explore debug log to understand operations
4. Test with different card types

### Advanced Usage
1. Test factory state detection with blank cards
2. Compare used vs. blank cards
3. Monitor real-time card events in debug log
4. Use diagnostics for troubleshooting

---

## Quick Reference Card

| Action | Steps |
|--------|-------|
| **First Read** | Place card ? Auto-connect ? Click Read |
| **Manual Connect** | Select reader ? Place card ? Click Connect |
| **View Logs** | Check Debug Log panel on right |
| **Diagnose Issues** | Click "PC/SC Diagnostics" button |
| **Clear Log** | Click "Clear Log" button |
| **Remove Card** | Just lift card (auto-disconnect) |

---

## Need Help?

1. **Check Debug Log** - Detailed error messages
2. **Run Diagnostics** - "PC/SC Diagnostics" button
3. **Verify Service** - Smart Card service must be running
4. **Test Hardware** - Try card on different device

---

**You're ready to go!** ??

Place a card and click Read to see the magic happen.

*Monroe NFC Card Configurator v1.0.0*  
*Developed by Monroe*
