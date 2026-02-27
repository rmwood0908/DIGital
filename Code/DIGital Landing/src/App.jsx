import { useState } from 'react'
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import './App.css'
import Game from './Game'
const digitalLogo = '/DIGitalLogo.png'
const nauLogo = '/NAULogo.png'

function LandingPage() {
  const [isMenuOpen, setIsMenuOpen] = useState(false)
  const { t, i18n } = useTranslation()

  const toggleLang = () => i18n.changeLanguage(i18n.language === 'en' ? 'es' : 'en')

  return (
    <div className="landing-page">
      <header className="navbar">
        <div className="container">
          <div className="logo">
            <img src={digitalLogo} alt="DIGital Logo" className="logo-image" />
          </div>
          <button className="menu-toggle" onClick={() => setIsMenuOpen(!isMenuOpen)}>☰</button>
          <nav className={isMenuOpen ? 'open' : ''}>
            <a href="#about">{t('nav.about')}</a>
            <Link to="/game">{t('nav.virtualExperience')}</Link>
            <a href="#quizzes">{t('nav.learningTools')}</a>
            <a href="#dr-sharp">{t('nav.meetDrSharp')}</a>
            <a href="#team">{t('nav.meetTeam')}</a>
            <button onClick={toggleLang} className="lang-toggle">
              {i18n.language === 'en' ? 'Español' : 'English'}
            </button>
          </nav>
        </div>
      </header>

      <section className="hero">
        <div className="container">
          <div className="hero-content">
            <h1 className="hero-title">{t('hero.title')}</h1>
            <p className="hero-subtitle">{t('hero.subtitle')}</p>
            <div className="hero-buttons">
              <Link to="/game" className="btn btn-primary">{t('hero.exploreNow')}</Link>
              <a href="#about" className="btn btn-secondary">{t('hero.learnMore')}</a>
            </div>
          </div>
          <div className="hero-visual">
            <img src={digitalLogo} alt="DIGital Logo" className="hero-logo" />
          </div>
        </div>
      </section>

      <section id="about" className="about">
        <div className="container">
          <h2>{t('about.heading')}</h2>
          <p className="section-subtitle">{t('about.subtitle')}</p>
          <div className="about-content">
            <div className="about-text">
              <p>{t('about.p1')}</p>
              <p>{t('about.p2')}</p>
            </div>
            <div className="about-stats">
              <div className="stat"><h3>∞</h3><p>{t('about.stat1')}</p></div>
              <div className="stat"><h3>↓</h3><p>{t('about.stat2')}</p></div>
              <div className="stat"><h3>✓</h3><p>{t('about.stat3')}</p></div>
            </div>
          </div>
        </div>
      </section>

      <section id="features" className="features">
        <div className="container">
          <h2>{t('features.heading')}</h2>
          <p className="section-subtitle">{t('features.subtitle')}</p>
          <div className="feature-grid">
            <div className="feature-card"><h3>{t('features.f1Title')}</h3><p>{t('features.f1Desc')}</p></div>
            <div className="feature-card"><h3>{t('features.f2Title')}</h3><p>{t('features.f2Desc')}</p></div>
            <div className="feature-card"><h3>{t('features.f3Title')}</h3><p>{t('features.f3Desc')}</p></div>
            <div className="feature-card"><h3>{t('features.f4Title')}</h3><p>{t('features.f4Desc')}</p></div>
            <div className="feature-card"><h3>{t('features.f5Title')}</h3><p>{t('features.f5Desc')}</p></div>
          </div>
        </div>
      </section>

      <section id="technology" className="technology">
        <div className="container">
          <h2>{t('technology.heading')}</h2>
          <div className="tech-grid">
            <div className="tech-card"><h3>{t('technology.t1Title')}</h3><p>{t('technology.t1Desc')}</p></div>
            <div className="tech-card"><h3>{t('technology.t2Title')}</h3><p>{t('technology.t2Desc')}</p></div>
            <div className="tech-card"><h3>{t('technology.t3Title')}</h3><p>{t('technology.t3Desc')}</p></div>
          </div>
        </div>
      </section>

      <section id="team" className="team">
        <div className="container">
          <h2>{t('team.heading')}</h2>
          <p className="section-subtitle">{t('team.subtitle')}</p>
          <div className="team-grid">
            <div className="team-member"><div className="member-avatar">DJ</div><h3>Devin Jay San Nicolas</h3><p>{t('team.dj')}</p></div>
            <div className="team-member"><div className="member-avatar">RW</div><h3>Ryan Wood</h3><p>{t('team.rw')}</p></div>
            <div className="team-member"><div className="member-avatar">TW</div><h3>Tate Whittaker</h3><p>{t('team.tw')}</p></div>
            <div className="team-member"><div className="member-avatar">JC</div><h3>Jarom Craghead</h3><p>{t('team.jc')}</p></div>
          </div>
          <div className="client-section">
            <h3>{t('team.collab')}</h3>
            <p className="client-name">Dr. Kayleigh Sharp</p>
            <p className="client-title">{t('team.drSharpTitle')}</p>
          </div>
        </div>
      </section>

      <section id="contact" className="cta-section">
        <div className="container">
          <h2>{t('cta.heading')}</h2>
          <p>{t('cta.body')}</p>
          <Link to="/game" className="btn btn-primary btn-large">{t('cta.btn')}</Link>
        </div>
      </section>

      <footer className="footer">
        <div className="container">
          <div className="footer-content">
            <div className="footer-section"><h4>{t('footer.title')}</h4></div>
            <div className="footer-section">
              <h4>{t('footer.quickLinks')}</h4>
              <ul>
                <li><a href="#about">{t('footer.about')}</a></li>
                <li><Link to="/game">{t('footer.playGame')}</Link></li>
                <li><a href="#quizzes">{t('footer.quizzes')}</a></li>
              </ul>
            </div>
            <div className="footer-section">
              <h4>{t('footer.connect')}</h4>
              <ul>
                <li><a href="https://github.com/Devin-Jay/DIGital" target="_blank" rel="noopener noreferrer">GitHub</a></li>
                <li><a href="https://sce.nau.edu/capstone/projects/CS/2026/DIGital_F25/" target="_blank" rel="noopener noreferrer">Team Website</a></li>
              </ul>
            </div>
            <div className="footer-section nau-section">
              <img src={nauLogo} alt="Northern Arizona University" className="nau-logo" />
            </div>
          </div>
          <div className="footer-bottom"><p>{t('footer.copyright')}</p></div>
        </div>
      </footer>
    </div>
  )
}

export default function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/game" element={<Game />} />
      </Routes>
    </Router>
  )
}
