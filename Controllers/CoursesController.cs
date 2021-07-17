using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course obj = new Course();
            obj.listcategory = context.Categories.ToList();
            return View(obj);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();
            ModelState.Remove("LecturerID");
            if (!ModelState.IsValid)
            {
                objCourse.listcategory = context.Categories.ToList();
                return View("Create", objCourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerID = user.Id;

            context.Courses.Add(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(objCourse.LecturerID).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Where(c => c.LecturerID == currentUser.Id && c.DateTime > DateTime.Now).ToList(); 
            foreach (Course i in courses)
            {
                i.Name = currentUser.Name;
            }
            return View(courses);
        }
        public ActionResult Edit(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course obj = new Course();
            obj.listcategory = context.Categories.ToList();
            var courses = context.Courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(context.Categories, "Id", "Name", courses.Id);
            return View(courses);
            
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course Course)
        {
            BigSchoolContext context = new BigSchoolContext();
            if (Course ==null)
            {
                return HttpNotFound();
            }
            var edit = context.Courses.Find(Course.Id);
            edit.Place = Course.Place;
            edit.DateTime = Course.DateTime;
            edit.CategoryID = Course.CategoryID;
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        public ActionResult Delete(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course obj = new Course();
            obj.listcategory = context.Categories.ToList();
            var courses = context.Courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(context.Categories, "Id", "Name", courses.Id);
            return View(courses);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course c = context.Courses.Find(id);
            var a = context.Attendances.Where(p => p.CourseId == c.Id && p.Attendee == c.LecturerID).ToList();
            if (a != null)
            {
                foreach (Attendance item in a)
                {
                    context.Attendances.Remove(item);
                    context.SaveChanges();
                }
            }
            context.Courses.Remove(c);
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        public ActionResult LectureIamGoing()
        {

            ApplicationUser currentUser =

            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); BigSchoolContext context = new BigSchoolContext();
            var listFollwee = context. Followings.Where (p => p. FollowerId ==currentUser.Id). ToList();
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerID)
                    {

                        Course objCourse = course.Course; 
                        objCourse.Name =
                       System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerID).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
            }
    }
    
}