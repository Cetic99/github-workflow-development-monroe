/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: ${(props) => (props.subtitle ? 'space-between' : 'flex-start')};
  gap: ${(props) => (props.hasGap ? '3rem' : '0')};
  user-select: none;
  cursor: ${(props) => (props.disabled ? 'not-allowed' : 'pointer')};
  transition: all 0.25s ease;

  padding: 1.25rem;
  border-radius: var(--border-radius);

  background-color: ${(props) => (!props.disabled ? props.color : 'var(--disabled-bg)')};

  &:active {
    background-color: ${(props) => props.color};
  }

  &:active {
    background-color: ${(props) => (!props.disabled ? props.activeColor : 'var(--disabled-bg)')};
  }

  & .card-button-header {
    line-height: 0;
  }

  & .card-button-content {
    margin-top: auto;
    font-size: 2.125rem;
    line-height: 2.375rem;
    font-weight: 600;
    color: ${(props) => props.textColor};
  }

  & .card-button-subtitle {
    color: var(--secondary-dark);
    font-size: 0.875rem;
    text-transform: uppercase;
    font-weight: 600;
    margin-top: 0.5rem;
  }

  & .card-button-overline {
    color: var(--primary-light);
    font-size: 0.875rem;
    font-weight: 600;
    text-transform: uppercase;
  }
`

const CardButton = ({
  icon,
  text,
  onClick,
  disabled = false,
  color = 'var(--secondary-dark)',
  activeColor = 'var(--primary-dark)',
  textColor = 'white',
  subtitle,
  overline,
  hasGap = true,
  className
}) => {
  const handleClick = (event) => {
    if (disabled) {
      event.stopPropagation()
      return
    }
    onClick(event)
  }
  return (
    <Container
      onClick={handleClick}
      disabled={disabled}
      color={color}
      activeColor={activeColor}
      textColor={textColor}
      className={className}
      hasGap={hasGap}
      subtitle={subtitle}
    >
      <div className="card-button-header">{icon}</div>
      {overline && <div className="card-button-overline">{overline}</div>}
      <div>
        <div className="card-button-content">{text}</div>
        {subtitle && <div className="card-button-subtitle">{subtitle}</div>}
      </div>
    </Container>
  )
}

export default CardButton
