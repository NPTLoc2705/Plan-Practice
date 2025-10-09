import React from 'react';
import { Link } from 'react-router-dom';

const Landing = () => {
  return (
    <div className="min-h-screen flex flex-col bg-gradient-to-br from-blue-50 via-cyan-50 to-white">
      {/* Header */}

      {/* Hero Section */}
      <section className="py-20 bg-gradient-to-br from-green-500 to-cyan-600 text-white relative overflow-hidden">
        <div className="container mx-auto px-6 grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
          <div>
            <h1 className="text-4xl md:text-5xl font-extrabold mb-6 drop-shadow">
              Revolutionize Your Learning Experience
            </h1>
            <p className="text-lg mb-8 opacity-90">
              Empower teachers to create engaging lessons and interactive quizzes. 
              Enable students to learn, practice, and excel in their studies.
            </p>
            <div className="flex flex-wrap gap-4">
              <a href="#register" className="bg-white text-blue-600 font-semibold px-8 py-4 rounded-xl shadow hover:-translate-y-1 hover:shadow-lg transition text-lg">
                Get Started Free
              </a>
              <a href="#features" className="bg-transparent border-2 border-white text-white font-semibold px-8 py-4 rounded-xl hover:bg-white hover:text-blue-600 transition text-lg">
                Learn More
              </a>
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div className="bg-white/20 backdrop-blur-lg p-8 rounded-2xl text-center border border-white/30 shadow-lg col-span-2 hover:-translate-y-2 transition">
              <div className="text-4xl mb-2">📚</div>
              <div className="font-semibold text-lg">Interactive Learning</div>
            </div>
            <div className="bg-white/20 backdrop-blur-lg p-8 rounded-2xl text-center border border-white/30 shadow-lg hover:-translate-y-2 transition">
              <div className="text-4xl mb-2">🎓</div>
              <div className="font-semibold text-lg">Expert Teachers</div>
            </div>
            <div className="bg-white/20 backdrop-blur-lg p-8 rounded-2xl text-center border border-white/30 shadow-lg hover:-translate-y-2 transition">
              <div className="text-4xl mb-2">📊</div>
              <div className="font-semibold text-lg">Track Progress</div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-24 bg-gradient-to-b from-gray-50 to-white">
        <div className="container mx-auto px-6">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-extrabold mb-4 text-gray-900">Why Choose PlanPractice?</h2>
            <p className="text-lg text-gray-600 max-w-xl mx-auto">
              Our platform bridges the gap between teaching and learning with powerful tools
            </p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* Teacher Features */}
            <div className="bg-white rounded-2xl p-8 shadow border hover:shadow-lg transition">
              <div className="flex items-center gap-4 mb-8">
                <div className="text-3xl bg-blue-100 p-4 rounded-xl shadow">👨‍🏫</div>
                <h3 className="text-xl font-bold text-gray-900">For Teachers</h3>
              </div>
              <div className="space-y-6">
                <div className="flex gap-4 items-start border-b pb-6">
                  <div className="text-2xl">📝</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Create Lessons</h4>
                    <p className="text-gray-600">Design comprehensive lessons with rich content, multimedia, and interactive elements</p>
                  </div>
                </div>
                <div className="flex gap-4 items-start border-b pb-6">
                  <div className="text-2xl">❓</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Build Quizzes</h4>
                    <p className="text-gray-600">Create engaging quizzes with multiple question types and instant feedback</p>
                  </div>
                </div>
                <div className="flex gap-4 items-start">
                  <div className="text-2xl">📈</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Track Performance</h4>
                    <p className="text-gray-600">Monitor student progress and identify areas that need attention</p>
                  </div>
                </div>
              </div>
            </div>
            {/* Student Features */}
            <div className="bg-white rounded-2xl p-8 shadow border hover:shadow-lg transition">
              <div className="flex items-center gap-4 mb-8">
                <div className="text-3xl bg-cyan-100 p-4 rounded-xl shadow">👨‍🎓</div>
                <h3 className="text-xl font-bold text-gray-900">For Students</h3>
              </div>
              <div className="space-y-6">
                <div className="flex gap-4 items-start border-b pb-6">
                  <div className="text-2xl">📖</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Interactive Learning</h4>
                    <p className="text-gray-600">Access engaging lessons designed to make learning enjoyable and effective</p>
                  </div>
                </div>
                <div className="flex gap-4 items-start border-b pb-6">
                  <div className="text-2xl">🎯</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Take Quizzes</h4>
                    <p className="text-gray-600">Test your knowledge with interactive quizzes and receive instant feedback</p>
                  </div>
                </div>
                <div className="flex gap-4 items-start">
                  <div className="text-2xl">🏆</div>
                  <div>
                    <h4 className="font-semibold text-lg mb-1">Achieve Goals</h4>
                    <p className="text-gray-600">Set learning goals and track your progress towards academic success</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* How It Works Section */}
      <section className="py-20 bg-white">
        <div className="container mx-auto px-6">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl font-extrabold mb-4 text-gray-900">How It Works</h2>
            <p className="text-lg text-gray-600">Get started in three simple steps</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center p-8 rounded-2xl shadow border hover:shadow-lg transition">
              <div className="w-16 h-16 mx-auto mb-6 flex items-center justify-center rounded-full bg-gradient-to-br from-blue-600 to-cyan-500 text-white text-2xl font-bold shadow-lg">1</div>
              <h3 className="font-semibold text-xl mb-2 text-gray-900">Sign Up</h3>
              <p className="text-gray-600">Create your account and choose your role - Teacher or Student</p>
            </div>
            <div className="text-center p-8 rounded-2xl shadow border hover:shadow-lg transition">
              <div className="w-16 h-16 mx-auto mb-6 flex items-center justify-center rounded-full bg-gradient-to-br from-blue-600 to-cyan-500 text-white text-2xl font-bold shadow-lg">2</div>
              <h3 className="font-semibold text-xl mb-2 text-gray-900">Create or Join</h3>
              <p className="text-gray-600">Teachers create lessons and quizzes. Students join classes and start learning</p>
            </div>
            <div className="text-center p-8 rounded-2xl shadow border hover:shadow-lg transition">
              <div className="w-16 h-16 mx-auto mb-6 flex items-center justify-center rounded-full bg-gradient-to-br from-blue-600 to-cyan-500 text-white text-2xl font-bold shadow-lg">3</div>
              <h3 className="font-semibold text-xl mb-2 text-gray-900">Learn & Grow</h3>
              <p className="text-gray-600">Engage with content, track progress, and achieve your learning goals</p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-24 bg-gradient-to-br from-green-500 to-cyan-600 text-white relative overflow-hidden">
        <div className="container mx-auto px-6">
          <div className="text-center">
            <h2 className="text-3xl md:text-4xl font-extrabold mb-4">Ready to Transform Your Learning Journey?</h2>
            <p className="text-lg opacity-90 mb-8">
              Join thousands of teachers and students already using PlanPractice
            </p>
            <div className="flex flex-wrap gap-4 justify-center">
              <a href="#register" className="bg-white text-blue-600 font-semibold px-8 py-4 rounded-xl shadow hover:-translate-y-1 hover:shadow-lg transition text-lg">
                Start Learning Today
              </a>
              <a href="#login" className="bg-transparent border-2 border-white text-white font-semibold px-8 py-4 rounded-xl hover:bg-white hover:text-blue-600 transition text-lg">
                Already have an account?
              </a>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-gray-900 text-white py-12 mt-auto">
        <div className="container mx-auto px-6 grid grid-cols-1 md:grid-cols-3 gap-8 mb-8">
          <div>
            <h3 className="text-2xl font-bold bg-gradient-to-r from-blue-400 to-cyan-400 bg-clip-text text-transparent mb-4">PlanPractice</h3>
            <p className="text-gray-400">Empowering education through innovative technology</p>
          </div>
          <div>
            <h4 className="text-lg font-semibold mb-4">Platform</h4>
            <ul>
              <li className="mb-2"><a href="#features" className="text-gray-400 hover:text-white transition">Features</a></li>
              <li className="mb-2"><a href="#pricing" className="text-gray-400 hover:text-white transition">Pricing</a></li>
              <li className="mb-2"><a href="#support" className="text-gray-400 hover:text-white transition">Support</a></li>
            </ul>
          </div>
          <div>
            <h4 className="text-lg font-semibold mb-4">Account</h4>
            <ul>
              <li className="mb-2"><a href="#login" className="text-gray-400 hover:text-white transition">Login</a></li>
              <li className="mb-2"><a href="#register" className="text-gray-400 hover:text-white transition">Sign Up</a></li>
              <li className="mb-2"><a href="#profile" className="text-gray-400 hover:text-white transition">Profile</a></li>
            </ul>
          </div>
        </div>
        <div className="border-t border-gray-800 pt-6 text-center text-gray-400">
          &copy; 2025 PlanPractice. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default Landing;
