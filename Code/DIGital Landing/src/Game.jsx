import { useEffect, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { LanguagePicker } from './App'
import './Game.css'

export default function Game() {
  const canvasRef = useRef(null)
  const [isLoading, setIsLoading] = useState(true)
  const [loadingProgress, setLoadingProgress] = useState(0)
  const [isFullscreen, setIsFullscreen] = useState(false)
  const [unityInstance, setUnityInstance] = useState(null)
  const { t } = useTranslation()

  useEffect(() => {
    const loadUnityGame = async () => {
      try {
        const buildUrl = '/DemoBuild'
        const config =
        {
          dataUrl: `${buildUrl}/WebGL.data.br`,
          frameworkUrl: `${buildUrl}/WebGL.framework.js.br`,
          codeUrl: `${buildUrl}/WebGL.wasm.br`,
          streamingAssetsUrl: `${buildUrl}/StreamingAssets`,
          companyName: 'DIGital',
          productName: 'Virtual Excavation',
          productVersion: '1.0',
        }

        // Load Unity loader script
        if (!window.createUnityInstance) {
          const script = document.createElement('script')
          script.src = `${buildUrl}/WebGL.loader.js`
          script.async = true
          document.body.appendChild(script)

          await new Promise((resolve) => {
            script.onload = resolve
          })
        }

        // Create Unity instance
        const instance = await window.createUnityInstance(canvasRef.current, config, (progress) => {
          setLoadingProgress(Math.round(progress * 100))
        })

        setUnityInstance(instance)
        setIsLoading(false)
      } catch (error) {
        console.error('Error loading Unity game:', error)
        setIsLoading(false)
      }
    }

    loadUnityGame()

    // Cleanup
    return () => {
      if (unityInstance) {
        unityInstance.Quit()
      }
    }
  }, [])

  const toggleFullscreen = () => {
    if (!isFullscreen) {
      unityInstance?.SetFullscreen(1)
      setIsFullscreen(true)
    } else {
      unityInstance?.SetFullscreen(0)
      setIsFullscreen(false)
    }
  }

  return (
    <div className="game-page">
      {/* Navigation Bar */}
      <nav className="game-nav">
        <a href="/" className="back-link">{t('game.backHome')}</a>
        <h1>{t('game.title')}</h1>
        <div className="nav-controls">
          <LanguagePicker />
          <button onClick={toggleFullscreen} className="fullscreen-btn">
            {isFullscreen ? t('game.exitFullscreen') : t('game.fullscreen')}
          </button>
        </div>
      </nav>

      {/* Game Container */}
      <div className="game-container">
        <div className="game-wrapper">
          {isLoading && (
            <div className="loading-overlay">
              <div className="loading-content">
                <div className="spinner"></div>
                <h2>{t('game.loadingTitle')}</h2>
                <div className="progress-bar">
                  <div
                    className="progress-fill"
                    style={{ width: `${loadingProgress}%` }}
                  ></div>
                </div>
                <p>{loadingProgress}%</p>
              </div>
            </div>
          )}

          <canvas
            ref={canvasRef}
            id="unity-canvas"
            className={isLoading ? 'hidden' : ''}
          ></canvas>
        </div>

        {/* Game Info Sidebar */}
        <div className="game-info">
          <div className="info-section">
            <h3>{t('game.controls')}</h3>
            <ul>
              <li><strong>WASD</strong> - {t('game.move')}</li>
              <li><strong>Mouse</strong> - {t('game.look')}</li>
            </ul>
          </div>

          <div className="info-section">
            <h3>{t('game.objectives')}</h3>
            <ul>
              <li>{t('game.obj1')}</li>
              <li>{t('game.obj2')}</li>
              <li>{t('game.obj3')}</li>
              <li>{t('game.obj4')}</li>
            </ul>
          </div>

          <div className="info-section">
            <h3>{t('game.tips')}</h3>
            <ul>
              <li>{t('game.tip1')}</li>
              <li>{t('game.tip2')}</li>
              <li>{t('game.tip3')}</li>
            </ul>
          </div>
        </div>
      </div>

      {/* Footer */}
      <footer className="game-footer">
        <p>
          {t('game.footerHelp')} <a href="/tutorial">{t('game.tutorial')}</a>
        </p>
      </footer>
    </div>
  )
}
