/*
 * Copyright (C) 2018 Aries Sanchez Sulit
 * Copyright (C) 2018 Synergy88 Digital.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import "DatePicker.h"
#import "DatePickerVC.h"
#import "DatePickerDialog.h"

@implementation DatePicker

#pragma mark -
#pragma mark Helpers
NSString* ToNSString(const char* p_str)
{
    if (p_str)
    {
        return [NSString stringWithUTF8String:p_str];
    }
    else
    {
        return [NSString stringWithUTF8String:""];
    }
}

#pragma mark -
#pragma mark Bridge
extern "C"
{
    void Select(
        const char* p_title,
        const char* p_done,
        const char* p_cancel,
        const char* p_gameObjectName,
        const char* p_methodName,
                int p_year,
                int p_month,
                int p_day
    ) {
        NSString* title = ToNSString(p_title);
        NSString* done = ToNSString(p_done);
        NSString* cancel = ToNSString(p_cancel);
        NSString* gameObject = ToNSString(p_gameObjectName);
        NSString* method = ToNSString(p_methodName);

        [DatePicker Select:title
                      Done:done
                    Cancel:cancel
                GameObject:gameObject
                    Method:method
                      Year:p_year
                     Month:p_month
                       Day:p_day];
    }
    
    bool AppIsInstalled(const char* p_package)
    {
        NSString* package = ToNSString(p_package);
        
        return [DatePicker AppIsInstalled:package];
    }
}

#pragma mark -
#pragma mark Objective-C Method Calls
+ (void) Select
{
    NSLog(@"Framework::Select Show the Fucking DatePicker here.");
}

+ (void) Select:(NSString*)p_title
           Done:(NSString*)p_done
         Cancel:(NSString*)p_cancel
     GameObject:(NSString*)p_gameObject
         Method:(NSString*)p_method
           Year:(int)p_year
          Month:(int)p_month
            Day:(int)p_day
{
    NSLog(@"Framework::Select Title:%@ Done:%@ Cancel:%@ GameObject:%@ Method:%@ %d:%d:%d",
          p_title,
          p_done,
          p_cancel,
          p_gameObject,
          p_method,
          p_year,
          p_month,
          p_day);
    
    // Selected date
    NSCalendar* calendar = [NSCalendar currentCalendar];
    NSDateComponents* components = [[NSDateComponents alloc] init];
    [components setDay:p_day];
    [components setMonth:p_month];
    [components setYear:p_year];
    NSDate* selected = [calendar dateFromComponents:components];
    
    // +AS:07202018 Custom dialog with parameters
    DatePickerDialog* dpDialog = [[DatePickerDialog alloc] init];
    [dpDialog showWithTitle:p_title
            doneButtonTitle:p_done
          cancelButtonTitle:p_cancel
                defaultDate:selected
                minimumDate:nil
                maximumDate:nil
             datePickerMode:UIDatePickerModeDate
                   callback:^(NSDate * _Nullable date)
     {
         if (date)
         {
             NSCalendar* calendar = [NSCalendar currentCalendar];
             NSDateComponents* components = [calendar components:NSCalendarUnitYear|NSCalendarUnitMonth|NSCalendarUnitDay fromDate:date];
             NSInteger year = [components year];
             NSInteger month = [components month];
             NSInteger day = [components day];
             NSString* dateString = [NSString stringWithFormat:@"%ld,%ld,%ld", (long)year, (long)month, (long)day];
             
             NSLog(@"Framework::Select Date:%@ Title:%@ Done:%@ Cancel:%@ GameObject:%@ Method:%@",
                   dateString,
                   p_title,
                   p_done,
                   p_cancel,
                   p_gameObject,
                   p_method);
             
             UnitySendMessage([p_gameObject UTF8String], [p_method UTF8String], [dateString UTF8String]);
             [dpDialog removeFromSuperview];
         }
     }];
}

+ (BOOL) AppIsInstalled:(NSString*)p_package
{
    BOOL isInstalled = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:p_package]];
    NSLog(@"Framework::AppIsInstalled Package:%@ IsInstalled:%hhd", p_package, isInstalled);
    return isInstalled;
}

@end
