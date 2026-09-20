# Testing Guide: Login Redirect Fix

## Quick Test (5 minutes)

### Prerequisites
✅ Backend running at `http://localhost:5000`
✅ Frontend running at `http://localhost:5173`  
✅ Database with test user (or register new account)

---

## Test 1: Login with Existing User

### Setup
Database should have a test user:
- Email: `test@example.com`
- Password: `Password123`

If not, register first (see Test 2).

### Steps

1. **Open Browser**
   - Navigate to: `http://localhost:5173`
   - Should show login page

2. **Fill Login Form**
   - Email: `test@example.com`
   - Password: `Password123`
   - Click "Sign In" button

3. **Verify Redirect**
   ✅ Page should redirect to `/records`
   ✅ URL should change to `http://localhost:5173/records`
   ✅ Header should appear with "Welcome, [Your Name]"
   ✅ Records list should load

### Expected Result
```
LoginPage
	↓ (after submit)
	↓ (wait 1-2 seconds)
RecordsPage ✅
```

---

## Test 2: Register New Account → Login

### Steps

1. **Go to Registration Page**
   - Start at: `http://localhost:5173`
   - Click "Create one" link
   - Should navigate to `/register`

2. **Fill Registration Form**
   - Name: `John Doe`
   - Email: `john@example.com`
   - Password: `TestPassword123`
   - Confirm Password: `TestPassword123`
   - Click "Create Account"

3. **Verify Redirect to Login**
   ✅ Should redirect to `/login` page
   ✅ URL should change to `http://localhost:5173/login`

4. **Login with New Account**
   - Email: `john@example.com`
   - Password: `TestPassword123`
   - Click "Sign In"

5. **Verify Redirect to Records**
   ✅ Page should redirect to `/records`
   ✅ Header should appear
   ✅ Welcome message shows your name

### Expected Result
```
RegisterPage (fill form)
	↓ (submit)
	↓
LoginPage ✅ (redirect)
	↓ (fill form)
	↓ (submit)
	↓
RecordsPage ✅ (redirect)
```

---

## Test 3: Protected Route Access

### Steps

1. **Logout** (if necessary)
   - If on RecordsPage, click "Sign Out" button
   - Should redirect to `/login`

2. **Try Direct Access**
   - Open new tab
   - Navigate to: `http://localhost:5173/records`
   - WITHOUT logging in

3. **Verify Protection**
   ✅ Should redirect to `/login` page  
   ✅ NOT to `/records`
   ✅ Cannot access records without auth

### Expected Result
```
Direct access to /records (not logged in)
	↓
ProtectedRoute checks: isAuthenticated = false
	↓
Redirect to /login ✅
```

---

## Test 4: Browser DevTools Checks

### Open Developer Tools
Press `F12` or `Ctrl+Shift+I`

### Check Console Tab
✅ No JavaScript errors
✅ No red X marks
✅ No warnings about auth

### Check Network Tab
```
When logging in, you should see:
1. POST /api/auth/login ✅
   - Status: 200
   - Response contains: token, user, id, name, email

2. GET /api/records ✅
   - Status: 200
   - Response contains: array of records (or empty [])

If you see:
❌ 401 Unauthorized - Token not being sent
❌ 404 Not Found - Endpoint wrong
❌ CORS error - Backend CORS misconfigured
```

### Check Application Storage
👉 DevTools → Application tab → Local Storage

Look for: `authToken`
```
Key: authToken
Value: eyJhbGciOiJIUzI1NiIs... (long JWT token)
```

✅ Token should be present after login
✅ Token should be cleared after logout

---

## Test 5: Loading State

### Steps

1. **Open DevTools**
   - DevTools → Network tab
   - Set throttling to "Slow 3G" (Simulate slow connection)

2. **Login**
   - Fill form with credentials
   - Click "Sign In"

3. **Watch Button State**
   ✅ Button should show "Signing in..." (text change)
   ✅ Button should be disabled (can't click multiple times)
   ✅ After 2-5 seconds, should redirect

### Expected Behavior
```
Before submit: "Sign In" button
	↓ (click)
During request: "Signing in..." button (disabled)
	↓ (backend processing)
After response: Redirect to RecordsPage ✅
```

---

## Test 6: Error Handling

### Test Invalid Credentials

1. **Attempt Login with Wrong Password**
   - Email: `test@example.com`
   - Password: `WrongPassword123` (incorrect)
   - Click "Sign In"

2. **Verify Error Message**
   ✅ Should see error: "Invalid email or password"
   ✅ Should NOT redirect to `/records`
   ✅ Should stay on `/login`

3. **Try Again with Correct Password**
   - Password: `Password123` (correct)
   - Click "Sign In"
   ✅ Should successfully redirect to `/records`

### Test Missing Fields

1. **Submit Form without Email**
   - Leave email blank
   - Enter password: `Password123`
   - Click "Sign In"

2. **Verify Validation Error**
   ✅ Should see error: "Email is required"
   ✅ No API request should be made
   ✅ Should stay on `/login`

---

## Test 7: Logout Flow

### Steps

1. **Login Successfully**
   - Should be on `/records` page
   - Header visible with "Sign Out" button

2. **Click Sign Out**
   - Click "Sign Out" button in header

3. **Verify Logout**
   ✅ Should redirect to `/login` page
   ✅ Header should disappear
   ✅ localStorage `authToken` should be cleared

4. **Try Direct Access to Records**
   - Navigate to: `http://localhost:5173/records`

5. **Verify Protection**
   ✅ Should redirect back to `/login`
   ✅ Cannot access protected routes after logout

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Stays on login after submit | Check browser console for errors, verify backend is running |
| CORS error | Verify backend has CORS enabled, check API URL in .env.development |
| "Invalid credentials" error | Use correct test account or register new one |
| Button stays "Signing in..." | Backend might not be responding, check if API is running |
| Token not in storage | Check DevTools → Application, verify login completed |
| Can access `/records` without login | ProtectedRoute might not be working, check App.tsx |

---

## Success Criteria Checklist

- [ ] Login with valid credentials redirects to `/records` ✅
- [ ] Registration redirects to `/login` ✅  
- [ ] Login after registration works ✅
- [ ] Direct access to `/records` redirects to `/login` ✅
- [ ] Logout clears token and redirects to `/login` ✅
- [ ] No console errors ✅
- [ ] Auth token in localStorage after login ✅
- [ ] Error messages display for invalid credentials ✅
- [ ] Form validation works ✅
- [ ] Loading state visible during request ✅

---

## Performance Notes

**Expected Load Times**:
- Login page load: < 1 second
- Login request processing: 1-2 seconds
- Redirect to records: Instant after auth completes
- Records page load: 1-3 seconds

**If times are much slower**:
- Check browser Network tab for slow requests
- Verify backend response times are acceptable
- Check for JavaScript errors in console

---

## Notes

✅ **The fix is working if**:
1. You redirect to `/records` after successful login
2. You stay on `/login` if login fails
3. You cannot access `/records` without being authenticated
4. Logout clears your session properly

❌ **Bug still exists if**:
1. You stay on `/login` after successful login
2. Page flickers or shows redirect loop
3. Error about page not found or redirect issues

---

## Support

If tests fail:
1. Check `BUG_FIX_LOGIN_REDIRECT.md` for technical details
2. Verify backend is running and responding
3. Check browser console for errors
4. Clear cache: DevTools → Application → Clear all
5. Restart both backend and frontend

---

**Test Date**: [Your Date]
**Browser**: [Your Browser]
**Status**: ✅ All Tests Passed / ❌ Issues Found

Report any issues with specific test case numbers above.
