import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

const resources = {
  en: {
    translation: {
      nav: {
        about: "About",
        virtualExperience: "Virtual Experience",
        learningTools: "Learning Tools",
        meetDrSharp: "Meet Dr. Sharp",
        meetTeam: "Meet The Team",
      },
      hero: {
        title: "Discover Archaeology Virtually",
        subtitle: "Explore virtual archaeological sites from anywhere. Experience immersive field school training without the cost or travel.",
        exploreNow: "Explore Now",
        learnMore: "Learn More",
      },
      about: {
        heading: "What is DIGital?",
        subtitle: "Transforming Archaeological Education",
        p1: "DIGital Virtual Excavation is a web-based platform designed to make archaeological field school training accessible, affordable, and engaging. Traditional field schools can cost thousands of dollars and require travel to remote sites. We're changing that.",
        p2: "Our platform allows students and educators to explore authentic archaeological sites in immersive virtual environments, replacing expensive traditional programs while maintaining educational integrity.",
        stat1: "Accessible Anywhere",
        stat2: "Will always be free",
        stat3: "Same Education Quality",
      },
      features: {
        heading: "Key Features",
        subtitle: "Everything you need for immersive archaeological education",
        f1Title: "Digital Archaeological Experiences",
        f1Desc: "Access immersive experiences directly from your browser.",
        f2Title: "Authentic Sites",
        f2Desc: "Explore meticulously recreated archaeological sites based on real excavations and research.",
        f3Title: "Learning Outcomes",
        f3Desc: "Monitor learning outcomes with built-in assessment tools and detailed progress reports.",
        f4Title: "AI-Powered Guidance",
        f4Desc: "Get intelligent assistance and contextual information about artifacts and excavation techniques.",
        f5Title: "Understand the Process",
        f5Desc: "Experience all steps of the archaeological process.",
      },
      technology: {
        heading: "DIGital is committed to bring a high-quality product",
        t1Title: "Unity WebGL",
        t1Desc: "High-performance 3D graphics rendered directly in the browser for immersive experiences.",
        t2Title: "React & Next.js",
        t2Desc: "Modern frontend framework for a responsive, fast, and intuitive user interface.",
        t3Title: "AI Integration",
        t3Desc: "Intelligent assistant to provide contextual guidance and enhance the learning experience.",
      },
      team: {
        heading: "Our Team",
        subtitle: "Computer Science students at Northern Arizona University",
        dj: "Team Lead",
        rw: "Frontend Development",
        tw: "Game Development",
        jc: "Backend & Integration",
        collab: "In Collaboration With",
        drSharpTitle: "Project Client & Archaeological Advisor",
      },
      cta: {
        heading: "Ready to Explore?",
        body: "Join us in revolutionizing archaeological education.",
        btn: "Start Your Excavation",
      },
      footer: {
        title: "DIGital a Virtual Excavation Experience",
        quickLinks: "Quick Links",
        about: "About",
        playGame: "Play Game",
        quizzes: "Quizzes",
        connect: "Connect",
        copyright: "© 2025 DIGital Virtual Excavation",
      },
    }
  },
  es: {
    translation: {
      nav: {
        about: "Acerca de",
        virtualExperience: "Experiencia Virtual",
        learningTools: "Herramientas de Aprendizaje",
        meetDrSharp: "Conoce a la Dra. Sharp",
        meetTeam: "Conoce al Equipo",
      },
      hero: {
        title: "Descubre la Arqueología Virtualmente",
        subtitle: "Explora sitios arqueológicos virtuales desde cualquier lugar. Experimenta la capacitación inmersiva de campo sin el costo ni los viajes.",
        exploreNow: "Explorar Ahora",
        learnMore: "Más Información",
      },
      about: {
        heading: "¿Qué es DIGital?",
        subtitle: "Transformando la Educación Arqueológica",
        p1: "DIGital Virtual Excavation es una plataforma web diseñada para hacer que la capacitación arqueológica de campo sea accesible, asequible y atractiva. Las escuelas de campo tradicionales pueden costar miles de dólares y requieren viajar a sitios remotos. Estamos cambiando eso.",
        p2: "Nuestra plataforma permite a estudiantes y educadores explorar sitios arqueológicos auténticos en entornos virtuales inmersivos, reemplazando programas tradicionales costosos mientras se mantiene la integridad educativa.",
        stat1: "Accesible en Cualquier Lugar",
        stat2: "Siempre será gratuito",
        stat3: "La Misma Calidad Educativa",
      },
      features: {
        heading: "Características Principales",
        subtitle: "Todo lo que necesitas para la educación arqueológica inmersiva",
        f1Title: "Experiencias Arqueológicas Digitales",
        f1Desc: "Accede a experiencias inmersivas directamente desde tu navegador.",
        f2Title: "Sitios Auténticos",
        f2Desc: "Explora sitios arqueológicos meticulosamente recreados basados en excavaciones e investigaciones reales.",
        f3Title: "Resultados de Aprendizaje",
        f3Desc: "Monitorea los resultados de aprendizaje con herramientas de evaluación integradas e informes de progreso detallados.",
        f4Title: "Guía con IA",
        f4Desc: "Obtén asistencia inteligente e información contextual sobre artefactos y técnicas de excavación.",
        f5Title: "Comprende el Proceso",
        f5Desc: "Experimenta todos los pasos del proceso arqueológico.",
      },
      technology: {
        heading: "DIGital se compromete a ofrecer un producto de alta calidad",
        t1Title: "Unity WebGL",
        t1Desc: "Gráficos 3D de alto rendimiento renderizados directamente en el navegador para experiencias inmersivas.",
        t2Title: "React & Next.js",
        t2Desc: "Marco de trabajo frontend moderno para una interfaz de usuario receptiva, rápida e intuitiva.",
        t3Title: "Integración de IA",
        t3Desc: "Asistente inteligente para proporcionar orientación contextual y mejorar la experiencia de aprendizaje.",
      },
      team: {
        heading: "Nuestro Equipo",
        subtitle: "Estudiantes de Ciencias de la Computación en la Universidad del Norte de Arizona",
        dj: "Líder del Equipo",
        rw: "Desarrollo Frontend",
        tw: "Desarrollo de Juegos",
        jc: "Backend e Integración",
        collab: "En Colaboración Con",
        drSharpTitle: "Cliente del Proyecto y Asesora Arqueológica",
      },
      cta: {
        heading: "¿Listo para Explorar?",
        body: "Únete a nosotros en la revolución de la educación arqueológica.",
        btn: "Comienza tu Excavación",
      },
      footer: {
        title: "DIGital una Experiencia Virtual de Excavación",
        quickLinks: "Enlaces Rápidos",
        about: "Acerca de",
        playGame: "Jugar",
        quizzes: "Cuestionarios",
        connect: "Conectar",
        copyright: "© 2025 DIGital Virtual Excavation",
      },
    }
  }
};

i18n.use(initReactI18next).init({
  resources,
  lng: 'en',
  fallbackLng: 'en',
  interpolation: { escapeValue: false },
});

export default i18n;
