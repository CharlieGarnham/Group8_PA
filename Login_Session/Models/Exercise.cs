using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Login_Session.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Exercise Name")]
        public string ExerciseName { get; set; }

        [Required]
        [Display(Name = "Number/Time of Reps")]
        public string RepNoTime { get; set; }

        [Required]
        [Display(Name = "Number of Sets")]
        public string SetNo { get; set; }

        [Required]
        [Display(Name = "Description of Exercise")]
        public string ExerciseDescription { get; set; }

        [Required]
        [Display(Name = "Area of Exercise")]
        public string ExerciseArea { get; set; }

    }

    public class ChooseExercise
    {
        [Required]
        [Display(Name = "Whole Body")]
        public string WholeBody { get; set; }

        [Required]
        [Display(Name = "Arm")]
        public string Arm { get; set; }

        [Required]
        [Display(Name = "Leg")]
        public string Leg { get; set; }

        [Required]
        [Display(Name = "Back")]
        public string Back { get; set; }

        [Required]
        [Display(Name = "Core")]
        public string Core { get; set; }

        [Required]
        [Display(Name = "Cardio")]
        public string Cardio { get; set; }

    }
}
